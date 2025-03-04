using static Nezu.Core.Helpers.Math;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        private void DecodeUnconditional(uint instruction)
        {
            uint group = (instruction >> 26) & 0b11;
            switch (group)
            {
                case 0b00:
                    if (IsBitSet(instruction, 16)) { /* Set endianness */ }
                    else { /* CPS */ }
                    return;

                case 0b01: /* Cache preload */ return;

                case 0b10:
                    uint innerGroup = (instruction >> 25) & 0b111;
                    switch (innerGroup)
                    {
                        case 0b100:
                            if (IsBitSet(instruction, 20)) { /* RFE */ }
                            else { /* Save return state */ }
                            return;

                        case 0b101: ARM_BLX_1(instruction); return;

                        case 0b110: /* Additional coproc double reg transfer */ return;

                        case 0b111:
                            if (IsBitSet(instruction, 24)) { /* Additional coproc reg transfer */ }
                            else { /* Undefined instruction*/ }
                            return;
                    }
                    return;
            }
        }
    }
}