using static Nezu.Core.Helpers.Math;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        private void ExecuteBranch(uint instruction)
        {
            // See A4.1.5
            int targetOffset = ExpandToInt32(instruction & 0x7FFFFF) * 4;

            uint pc = Registers[15];
            bool link = IsBitSet(instruction, 24);
            if (link)
                Registers[14] = pc;

            // TODO: Verify this arithmetic
            Registers[15] = (uint)(pc + targetOffset + 4);
        }
    }
}