using Nezu.Core.Enums;
using System.Runtime.CompilerServices;

namespace Nezu.Core.ARM11
{
    /// <summary>
    /// Defines the ARM11 register set, with support for mode banking.
    /// </summary>
    public unsafe struct RegisterSet
    {
        private readonly Dictionary<Mode, uint[]> _bankedRegisters = new();
        private readonly Dictionary<Mode, uint> _bankedSpsr = new();

        private uint[] _registers = new uint[16];

        private Mode _currentMode;

        public RegisterSet()
        {
            _bankedRegisters[Mode.FIQ] = new uint[7];
            _bankedRegisters[Mode.User] = new uint[7];  // System mode shares this
            _bankedRegisters[Mode.Supervisor] = new uint[2];
            _bankedRegisters[Mode.Abort] = new uint[2];
            _bankedRegisters[Mode.IRQ] = new uint[2];
            _bankedRegisters[Mode.Undefined] = new uint[2];

            _bankedSpsr[Mode.FIQ] = 0;
            _bankedSpsr[Mode.Supervisor] = 0;
            _bankedSpsr[Mode.Abort] = 0;
            _bankedSpsr[Mode.IRQ] = 0;
            _bankedSpsr[Mode.Undefined] = 0;

            CPSR = 0;
            _currentMode = Mode.User;
            UpdateMode(Mode.User);
        }

        public uint this[int index]
        {
            get => _registers[index];
            set => _registers[index] = value;
        }

        /// <summary>
        /// Switches the register bank that the processor is using based on the requested <see cref="Mode"/>.
        /// </summary>
        /// <param name="newMode">The mode to set the processor to.</param>
        public void UpdateMode(Mode newMode)
        {
            bool isUserOrSystem = _currentMode == Mode.User || _currentMode == Mode.System;
            Mode effectiveMode = (_currentMode == Mode.System) ? Mode.User : _currentMode;
            if (_bankedRegisters.ContainsKey(effectiveMode))
            {
                int offset = (effectiveMode == Mode.FIQ) ? 8 : 13;
                int length = (effectiveMode == Mode.FIQ) ? 7 : 2;
                Buffer.BlockCopy(_registers, offset * sizeof(uint), _bankedRegisters[effectiveMode], 0, length * sizeof(uint));

                if (!isUserOrSystem)
                    _bankedSpsr[effectiveMode] = SPSR;
            }

            bool isNewUserOrSystem = newMode == Mode.User || newMode == Mode.System;
            Mode newEffectiveMode = (newMode == Mode.System) ? Mode.User : newMode;
            if (_bankedRegisters.ContainsKey(newEffectiveMode))
            {
                int offset = (newEffectiveMode == Mode.FIQ) ? 8 : 13;
                int length = (newEffectiveMode == Mode.FIQ) ? 7 : 2;
                Buffer.BlockCopy(_bankedRegisters[newEffectiveMode], 0, _registers, offset * sizeof(uint), length * sizeof(uint));

                if (!isNewUserOrSystem)
                    SPSR = _bankedSpsr[newEffectiveMode];
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
        public void SetFlag(Flag flag) => CPSR |= (uint)flag;

        /// <summary>
        /// Clears the<paramref name="flag"/> in the <see cref="CPSR"/>.
        /// </summary>
        /// <param name="flag">The flag to clear.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearFlag(Flag flag) => CPSR &= ~(uint)flag;

        /// <summary>
        /// Conditionally sets the specified <paramref name="flag"/> in the <see cref="CPSR"/> based on <paramref name="condition"/>.
        /// </summary>
        /// <param name="flag">The flag to modify.</param>
        /// <param name="condition">True to set the flag; false if otherwise.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ModifyFlag(Flag flag, bool condition) { if (condition) CPSR |= (uint)flag; else CPSR &= ~(uint)flag; }

        /// <summary>
        /// Determines whether a specified flag is set in the <see cref="CPSR"/>
        /// </summary>
        /// <param name="flag">The flag to check.</param>
        /// <returns>True if the flag is set; false if otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsFlagSet(Flag flag) => (CPSR & (uint)flag) != 0;
    }
}