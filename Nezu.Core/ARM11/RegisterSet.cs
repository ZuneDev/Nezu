using Nezu.Core.Enums;
using System.Runtime.CompilerServices;

namespace Nezu.Core.ARM11
{
    /// <summary>
    /// Defines the ARM11 register set, with support for mode banking.
    /// </summary>
    public unsafe struct RegisterSet
    {
        private uint _bankedSpsrFIQ;
        private uint _bankedSpsrSupervisor;
        private uint _bankedSpsrAbort;
        private uint _bankedSpsrIRQ;
        private uint _bankedSpsrUndefined;

        private uint[] _registers = new uint[16];

        private uint[] _bankedFIQ = new uint[7];
        private uint[] _bankedUser = new uint[7];
        private uint[] _bankedSupervisor = new uint[2];
        private uint[] _bankedAbort = new uint[2];
        private uint[] _bankedIRQ = new uint[2];
        private uint[] _bankedUndefined = new uint[2];

        private ARMMode _currentMode;

        public RegisterSet()
        {
            CPSR = 0;
            _currentMode = ARMMode.User;
            UpdateMode(ARMMode.User);
        }

        public uint this[int index]
        {
            get => _registers[index];
            set => _registers[index] = value;
        }

        /// <summary>
        /// Switches the register bank that the processor is using based on the requested <see cref="ARMMode"/>.
        /// </summary>
        /// <param name="newMode">The mode to set the processor to.</param>
        public void UpdateMode(ARMMode newMode)
        {
            switch (_currentMode)
            {
                case ARMMode.System:
                case ARMMode.User:
                    Array.Copy(_registers, 8, _bankedUser, 0, 7);
                    break;
                case ARMMode.FIQ:
                    Array.Copy(_registers, 8, _bankedFIQ, 0, 7);
                    _bankedSpsrFIQ = SPSR;
                    break;
                case ARMMode.Supervisor:
                    Array.Copy(_registers, 13, _bankedSupervisor, 0, 2);
                    _bankedSpsrSupervisor = SPSR;
                    break;
                case ARMMode.Abort:
                    Array.Copy(_registers, 13, _bankedAbort, 0, 2);
                    _bankedSpsrAbort = SPSR;
                    break;
                case ARMMode.IRQ:
                    Array.Copy(_registers, 13, _bankedIRQ, 0, 2);
                    _bankedSpsrIRQ = SPSR;
                    break;
                case ARMMode.Undefined:
                    Array.Copy(_registers, 13, _bankedUndefined, 0, 2);
                    _bankedSpsrUndefined = SPSR;
                    break;
            }

            if (newMode == ARMMode.FIQ) { Array.Copy(_bankedFIQ, 0, _registers, 8, 7); SPSR = _bankedSpsrFIQ; }
            else Array.Copy(_bankedUser, 0, _registers, 8, 7);

            switch (newMode)
            {
                case ARMMode.Supervisor:
                    Array.Copy(_bankedSupervisor, 0, _registers, 13, 2);
                    SPSR = _bankedSpsrSupervisor;
                    break;
                case ARMMode.Abort:
                    Array.Copy(_bankedAbort, 0, _registers, 13, 2);
                    SPSR = _bankedSpsrAbort;
                    break;
                case ARMMode.IRQ:
                    Array.Copy(_bankedIRQ, 0, _registers, 13, 2);
                    SPSR = _bankedSpsrIRQ;
                    break;
                case ARMMode.Undefined:
                    Array.Copy(_bankedUndefined, 0, _registers, 13, 2);
                    SPSR = _bankedSpsrUndefined;
                    break;
            }

            _currentMode = newMode;
        }

        /// <summary>The saved program status register.</summary>
        public uint SPSR;

        /// <summary>The current program status register.</summary>
        public uint CPSR;

        /// <summary>
        /// Sets the specified <paramref name="flag"/> in the <see cref="CPSR"/>.
        /// </summary>
        /// <param name="flag">The flag to set.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlag(ARMFlag flag) => CPSR |= (uint)flag;

        /// <summary>
        /// Clears the<paramref name="flag"/> in the <see cref="CPSR"/>.
        /// </summary>
        /// <param name="flag">The flag to clear.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearFlag(ARMFlag flag) => CPSR &= ~(uint)flag;

        /// <summary>
        /// Conditionally sets the specified <paramref name="flag"/> in the <see cref="CPSR"/> based on <paramref name="condition"/>.
        /// </summary>
        /// <param name="flag">The flag to modify.</param>
        /// <param name="condition">True to set the flag; false if otherwise.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ModifyFlag(ARMFlag flag, bool condition) { if (condition) CPSR |= (uint)flag; else CPSR &= ~(uint)flag; }

        /// <summary>
        /// Determines whether a specified flag is set in the <see cref="CPSR"/>
        /// </summary>
        /// <param name="flag">The flag to check.</param>
        /// <returns>True if the flag is set; false if otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsFlagSet(ARMFlag flag) => (CPSR & (uint)flag) != 0;
    }
}