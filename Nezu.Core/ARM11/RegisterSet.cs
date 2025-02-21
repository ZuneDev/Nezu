using Nezu.Core.Enums;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Nezu.Core.ARM11
{
    /// <summary>
    /// Defines the ARM11 register set, with support for register banking based on the current mode.
    /// </summary>
    public struct RegisterSet
    {
        [InlineArray(16)]
        private struct RegisterArray
        {
            private uint _r0;
            internal static int Length => 16;
        }

        // In reality, we have 6 distinct modes, but we are once again indexing based on the mode,
        // meaning we have 2^5 possible indices. Masking out the MSB could bring us down to 2^4,
        // but it's really not worth it to add more logic to the mode switch.
        private readonly BankParameters[] bankParams = new BankParameters[32];

        private uint[] _bankStore = new uint[22];  // USR/SYS (7 regs, default set), FIQ (7 regs), other 4 modes (2 regs ea.)
        private uint[] _bankedSpsr = new uint[6];  // 6 distinct modes, usr/sys don't use SPSR, but our bank switch relies on it anyways.
        private RegisterArray _registers = new RegisterArray();
        private Mode _currentMode;

        public RegisterSet()
        {
            bankParams[(uint)Mode.User] =       new BankParameters(8, 0, 7, 0);
            bankParams[(uint)Mode.System] =     new BankParameters(8, 0, 7, 0);
            bankParams[(uint)Mode.FIQ] =        new BankParameters(8, 7, 7, 1);
            bankParams[(uint)Mode.IRQ] =        new BankParameters(13, 14, 2, 2);
            bankParams[(uint)Mode.Supervisor] = new BankParameters(13, 16, 2, 3);
            bankParams[(uint)Mode.Abort] =      new BankParameters(13, 18, 2, 4);
            bankParams[(uint)Mode.Undefined] =  new BankParameters(13, 20, 2, 5);

            _currentMode = Mode.User;
            SwitchMode(Mode.User);
        }

        private unsafe ref uint GetRegisterRef(int index)
        {
            Debug.Assert(index >= 0 && index < 16);
            ref uint first = ref Unsafe.AsRef<uint>(Unsafe.AsPointer(ref _registers));
            return ref Unsafe.Add(ref first, index);
        }

        public uint this[int index]
        {
            get => GetRegisterRef(index);
            set => GetRegisterRef(index) = value;
        }

        public uint this[uint index]
        {
            get => GetRegisterRef((int)index);
            set => GetRegisterRef((int)index) = value;
        }

        // Some shortcuts to make the code a bit cleaner
        public const int LR = 14;
        public const int PC = 15;


        /// <summary>
        /// Switches the register bank that the processor is using based on the requested <see cref="Mode"/>.
        /// </summary>
        /// <param name="newMode">The mode to set the processor to.</param>
        public void SwitchMode(Mode newMode)
        {
            // Don't copy anything unless we have to
            if (_currentMode == newMode) return;

            // Copy state into respective bank
            BankParameters currentCopy = bankParams[(uint)_currentMode];
            for (int i = 0; i < currentCopy.RegisterCount; i++)
                _bankStore[currentCopy.BankIndex + i] = _registers[currentCopy.ActiveSetIndex + i];

            SPSR = CPSR;
            _bankedSpsr[currentCopy.SPSRIndex] = SPSR;

            // Copy in r8-r12 from the user bank if we're leaving FIQ and entering anything except for USR/SYS.
            // We do not need to copy r13 or r14 because every other mode overwrites them anyways.
            if (_currentMode is Mode.FIQ && (newMode is not Mode.User && newMode is not Mode.System))
                for (int i = 0; i < 5; i++) _registers[8 + i] = _bankStore[i];

            // Copy new bank into working set
            BankParameters newCopy = bankParams[(uint)newMode];
            for (int i = 0; i < newCopy.RegisterCount; i++) 
                _registers[newCopy.ActiveSetIndex + i] = _bankStore[newCopy.BankIndex + i];

            SPSR = _bankedSpsr[newCopy.SPSRIndex];
            CPSR = SPSR;

            _currentMode = newMode;
        }

        public void PrintRegisters()
        {
            Console.WriteLine($"Mode: {_currentMode}");
            for (int i = 0; i < RegisterArray.Length; i++)
            {
                Console.WriteLine($"R{i}: 0x{_registers[i]:X8}");
            }
            Console.WriteLine($"CPSR: 0x{CPSR:X8}");
            Console.WriteLine($"SPSR: 0x{SPSR:X8}");
            Console.WriteLine(new string('-', 30));
        }


        /// <summary>The saved program status register.</summary>
        public uint SPSR;

        /// <summary>The current program status register.</summary>
        public uint CPSR;

        #region Program status utilities
        /// <summary>
        /// Sets the specified <paramref name="flag"/> in the <see cref="CPSR"/>.
        /// </summary>
        /// <param name="flag">The flag to set.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlag(Flag flag) => CPSR |= (uint)flag;

        /// <summary>
        /// Clears the <paramref name="flag"/> in the <see cref="CPSR"/>.
        /// </summary>
        /// <param name="flag">The flag to clear.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearFlag(Flag flag) => CPSR &= ~(uint)flag;

        /// <summary>
        /// Conditionally sets the specified <paramref name="flag"/> in the <see cref="CPSR"/> based on <paramref name="condition"/>.
        /// </summary>
        /// <param name="flag">The flag to modify.</param>
        /// <param name="condition">True to set the flag; false otherwise.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ModifyFlag(Flag flag, bool condition) { if (condition) CPSR |= (uint)flag; else CPSR &= ~(uint)flag; }

        /// <summary>
        /// Conditionally sets <paramref name="flag"/> in the <see cref="CPSR"/> based on <paramref name="condition"/>.
        /// </summary>
        /// <param name="condition">
        /// <see cref="FlagResult.Set"/> to set the flag;
        /// <see cref="FlagResult.Unset"> to unset the flag;
        /// <see cref="FlagResult.Pass"/> to leave the flag as-is.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ModifyFlag(Flag flag, FlagResult condition)
        {
            if (condition is not FlagResult.Pass)
                ModifyFlag(flag, condition is FlagResult.Set);
        }

        /// <summary>
        /// Determines whether a specified flag is set in the <see cref="CPSR"/>
        /// </summary>
        /// <param name="flag">The flag to check.</param>
        /// <returns>True if the flag is set; false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsFlagSet(Flag flag) => (CPSR & (uint)flag) != 0;
        #endregion
    }


    /// <summary>
    /// Represents the parameters for a specific register bank used in different processor modes.
    /// This struct contains the information needed to switch between modes.
    /// </summary>
    readonly struct BankParameters
    {
        /// <summary>
        /// The index in the active register set where the registers for this mode start.
        /// </summary>
        internal readonly int ActiveSetIndex;

        /// <summary>
        /// The index in the bank storage where the registers for this mode are stored.
        /// </summary>
        internal readonly int BankIndex;

        /// <summary>
        /// The number of registers banked in this mode.
        /// </summary>
        internal readonly int RegisterCount;

        /// <summary>
        /// The index in the SPSR bank where the SPSR for this mode is stored.
        /// </summary>
        internal readonly int SPSRIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="BankParameters"/> struct with the specified values.
        /// </summary>
        /// <param name="activeSetIndex">The index in the active register set where the registers for this mode start.</param>
        /// <param name="bankIndex">The index in the banked storage array where the registers for this mode are stored.</param>
        /// <param name="registerCount">The number of registers used for this mode.</param>
        /// <param name="spsrIndex">The index in the SPSR bank array where the SPSR for this mode is stored.</param>
        internal BankParameters(int activeSetIndex, int bankIndex, int registerCount, int spsrIndex)
        {
            ActiveSetIndex = activeSetIndex;
            BankIndex = bankIndex;
            RegisterCount = registerCount;
            SPSRIndex = spsrIndex;
        }
    }
}