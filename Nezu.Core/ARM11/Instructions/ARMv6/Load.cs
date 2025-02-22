using Nezu.Core.Enums;
using System.Numerics;
using System.Runtime.CompilerServices;

using static Nezu.Core.Helpers.Math;
using static Nezu.Core.ARM11.RegisterSet;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        private void ARM_LDM(uint instruction)
        {
            ushort regList = (ushort)(instruction);
            bool r15 = (regList & 0x8000) != 0;
            uint rn = (instruction >> 16) & 0xF;
            bool usesMode = IsBitSet(instruction, 22);

            Mode currentMode = Registers.CurrentMode;
            if (usesMode && !r15) Registers.SwitchMode(Mode.User);

            uint addr = GetBaseLDSTMAddress(rn, regList, instruction);

            ref uint regBase = ref Registers.GetRegisterRef(0);
            while (regList != 0)
            {
                int index = BitOperations.TrailingZeroCount(regList);
                Unsafe.Add(ref regBase, index) = Memory.ReadWord(addr);
                addr += 4;
                regList &= (ushort)~(1 << index);
            }

            Registers.SwitchMode(currentMode);

            if (r15)
            {
                uint newPC = Memory.ReadWord(addr);
                Registers[PC] = newPC & 0xFFFFFFFE;
                if ((newPC & 1) != 0) SetModeThumb();
                if (usesMode) Registers.CPSR = Registers.SPSR;
            }
        }
    }
}