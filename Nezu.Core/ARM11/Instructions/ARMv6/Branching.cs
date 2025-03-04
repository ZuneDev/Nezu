using Nezu.Core.Enums;
using System.Numerics;
using System.Runtime.CompilerServices;

using static Nezu.Core.Helpers.Math;
using static Nezu.Core.ARM11.RegisterSet;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        private void ARM_BLX_1(uint instruction)
        {
            int target = ExpandToInt32(instruction & 0xFFFFFF);
            uint h = (instruction >> 24) & 1;
            uint pc = Registers[PC];

            Registers[LR] = pc + 4;
            SetModeThumb();
            Registers[PC] = (uint)(pc + (target << 2) + (h << 1));
        }

        private void ARM_BLX_2(uint instruction)
        {
            uint target = Registers[instruction & 0b1111];

            Registers[LR] = Registers[PC] + 4;
            Registers[PC] = target & 0xFFFFFFFE;

            if ((target & 1) != 0)
                SetModeThumb();
            else
                SetModeARM();
        }

        private void ARM_BX(uint instruction)
        {
            uint target = Registers[instruction & 0b1111];
            Registers[PC] = target & 0xFFFFFFFE;

            if ((target & 1) != 0)
                SetModeThumb();
            else
                SetModeARM();
        }

        private void ARM_B_L(uint instruction)
        {
            int target = ExpandToInt32(instruction & 0xFFFFFF);
            bool link = IsBitSet(instruction, 24);
            uint pc = Registers[PC];

            if (link) Registers[LR] = pc + 4;
            Registers[PC] = (uint)(pc + (target << 2));
        }
    }
}