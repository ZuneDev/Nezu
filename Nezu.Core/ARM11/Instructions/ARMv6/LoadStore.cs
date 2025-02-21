namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        private void ARM_LDM(uint instruction)
        {
            // TODO: finish after class
            ushort regList = (ushort)(instruction);
            bool r15 = (regList & 0x8000) != 0;
            uint rn = (instruction >> 16) & 0xF;
        }
    }
}