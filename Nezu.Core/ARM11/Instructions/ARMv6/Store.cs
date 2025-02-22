using Nezu.Core.Enums;
using System.Numerics;
using System.Runtime.CompilerServices;

using static Nezu.Core.Helpers.Math;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        private void ARM_STM(uint instruction)
        {
            ushort regList = (ushort)(instruction);
            uint rn = (instruction >> 16) & 0xF;
            bool usesMode = IsBitSet(instruction, 22);

            Mode currentMode = Registers.CurrentMode;
            if (usesMode) Registers.SwitchMode(Mode.User);

            uint addr = GetBaseLDSTMAddress(rn, regList, instruction);

            ref uint regBase = ref Registers.GetRegisterRef(0);
            while (regList != 0)
            {
                int index = BitOperations.TrailingZeroCount(regList);
                Memory.WriteWord(addr, Unsafe.Add(ref regBase, index));
                addr += 4;
                regList &= (ushort)~(1 << index);
            }

            Registers.SwitchMode(currentMode);
        }
    }
}