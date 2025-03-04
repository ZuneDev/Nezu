using Nezu.Core.Enums;
using System.Diagnostics;
using static Nezu.Core.Helpers.Math;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        private void DecodeAndExecuteARM(uint instruction)
        {
            var conditionCode = (ConditionCode)(instruction >> 28);
            if (!IsConditionSet(conditionCode))
                return;

            // See ARMv6 manual, Figure A3-1

            // Special unconditional instructions, see Figure A3-6
            if (conditionCode is ConditionCode.UD)
                DecodeUnconditional(instruction);

            uint group = (instruction >> 25) & 0b111;
            uint opcode;
            switch (group)
            {
                case 0b000:
                    if ((instruction & 0x90) == 0x90)
                    {
                        if (IsBitSet(instruction, 24) || ((instruction >> 5) & 0b11) != 0)
                            DecodeExtraLoadStore(instruction);
                        else
                            DecodeMultInstruction(instruction);
                    }
                    else
                    {
                        opcode = (instruction >> 21) & 0b1111;
                        if (!IsBitSet(instruction, 20) && (opcode >> 2) == 0b10)
                            DecodeMiscInstruction(instruction);
                        else
                            DecodeDataProcessing(instruction);
                    }
                    break;

                case 0b001:
                    opcode = (instruction >> 21) & 0b1111;
                    if (!IsBitSet(instruction, 20) && (opcode >> 2) == 0b10)
                    {
                        // Assert not undefined instruction
                        Debug.Assert((opcode & 1) != 0);
                        DecodeMoveSR(instruction);
                    }
                    else
                        DecodeDataProcessing(instruction);
                    break;

                case 0b010: DecodeLoadStoreOffset(instruction, false); break;

                case 0b011:
                    if (IsBitSet(instruction, 4))
                    {
#if DEBUG
                        // Assert not architecturally undefined instruction
                        bool higherUnset = ((~instruction >> 20) & 0b11111) != 0;
                        bool lowerUnset = ((~instruction >> 4) & 0b1111) != 0;
                        Debug.Assert(higherUnset || lowerUnset);
#endif
                        DecodeMediaInstruction(instruction);
                    }
                    else
                    {
                        DecodeLoadStoreOffset(instruction, true);
                    }
                    break;

                // Load/Store multiple: See A3-12
                case 0b100:
                    if (IsBitSet(instruction, 20)) ARM_LDM(instruction);
                    else ARM_STM(instruction);
                    break;

                // Handles both B and BL
                case 0b101: ARM_B_L(instruction); break;

                case 0b110: DecodeCoprocessorTransfer(instruction); break;

                case 0b111:
                    if (IsBitSet(instruction, 24))
                    {
                        // call SWI impl directly. there is no more decoding to be done; SWI is a single instruction.
                    }
                    else
                        DecodeCoprocessorInstruction(instruction);
                    break;

                default:
                    throw new NotImplementedException($"Unrecognized instruction: 0x{instruction:X8}");
            }
        }
    }
}