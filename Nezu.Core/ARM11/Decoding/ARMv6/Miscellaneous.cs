using static Nezu.Core.Helpers.Math;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        private void DecodeMiscInstruction(uint instruction)
        {
            if (IsBitSet(instruction, 7))
            {
                switch ((instruction >> 21) & 0b11)
                {
                    case 0b00: /* SMLA */ break;
                    case 0b01:
                        if (IsBitSet(instruction, 5)) { /* SMULW */ }
                        else { /* SMLAW */ }
                        break;
                    case 0b10: /* SMLAL */ break;
                    case 0b11: /* SMUL */ break;
                }
                return;
            }


            switch ((instruction >> 4) & 0xF)
            {
                case 0b0000:
                    if (IsBitSet(instruction, 21)) { /* MSR */ }
                    else { /* MRS */ }
                    break;

                case 0b0001:
                    if (IsBitSet(instruction, 22)) { /* CLZ */ }
                    else { /* BX */ }
                    break;

                case 0b0010: /* BXJ */ break;

                case 0b0011: /* BLX_2 */ break;

                case 0b0101:
                    switch ((instruction >> 21) & 0b11)
                    {
                        case 0b00: /* QADD */ break;
                        case 0b01: /* QSUB */ break;
                        case 0b10: /* QDADD */ break;
                        case 0b11: /* QDSUB */break;
                    }
                    break;

                case 0b0111: /* BKPT */ break;
            }
        }
    }
}