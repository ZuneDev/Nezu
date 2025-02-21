namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        private void DecodeCoprocessorInstruction(uint instruction)
        {
            if ((instruction & 0b10000) != 0)
            {
                // Coproc register transfer
            }
            else
            {
                // Coproc DP
            }
        }

        private void DecodeCoprocessorTransfer(uint instruction)
        {
            throw new NotImplementedException();
        }
    }
}