using static Nezu.Core.Helpers.Math;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        private void ExecuteDataProcessing(uint instruction)
        {
            // See A5.1
            bool immediate = IsBitSet(instruction, 25);
            uint opcode = (instruction >> 21) & 0b1111;
            bool S = IsBitSet(instruction, 20);
            uint Rn = (instruction >> 16) & 0b1111;
            uint Rd = (instruction >> 12) & 0b1111;

            uint value;

            // TODO: Set C flag
            if (immediate)
            {
                uint rotateValue = (instruction >> 8) & 0b1111;
                uint immediateValue = instruction & 0xFF;
                value = uint.RotateRight(immediateValue, (int)(rotateValue * 2));
            }
        }
    }
}