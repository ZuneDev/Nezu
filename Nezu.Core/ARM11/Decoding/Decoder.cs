﻿using Nezu.Core.Enums;
using System.Diagnostics;
using System.Reflection.Emit;
using static Nezu.Core.Helpers.Math;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        private void DecodeAndExecute(uint instruction)
        {
            var conditionCode = (ConditionCode)(instruction >> 28);
            if (!IsConditionSet(conditionCode))
                return;

            // See ARMv6 manual, Figure A3-1

            // Special unconditional instructions, see Figure A3-6
            if (conditionCode is ConditionCode.UD)
                ExecuteUnconditional(instruction);

            uint group = (instruction >> 25) & 0b111;
            switch (group)
            {
                case 0b000:
                    uint opcode = (instruction >> 21) & 0b1111;
                    bool s = IsBitSet(instruction, 20);
                    if ((opcode >> 2) == 0b10 && !s)
                        ExecuteMiscInstruction(instruction);
                    else
                        ExecuteDataProcessing(instruction);
                    break;

                case 0b001:
                    opcode = (instruction >> 21) & 0b1111;
                    s = IsBitSet(instruction, 20);
                    if ((opcode >> 2) == 0b10 && !s)
                    {
                        // Assert not undefined instruction
                        Debug.Assert((opcode & 1) != 0);

                        ExecuteMoveSR(instruction);
                    }
                    else
                        ExecuteDataProcessing(instruction);
                    break;

                case 0b010:
                    ExecuteLoadStoreOffset(instruction, false);
                    break;

                case 0b011:
                    if (IsBitSet(instruction, 4))
                    {
#if DEBUG
                        // Assert not architecturally undefined instruction
                        bool higherUnset = ((~instruction >> 20) & 0b11111) != 0;
                        bool lowerUnset = ((~instruction >> 4) & 0b1111) != 0;
                        Debug.Assert(higherUnset || lowerUnset);
#endif
                        ExecuteMediaInstruction(instruction);
                    }
                    else
                    {
                        ExecuteLoadStoreOffset(instruction, true);
                    }
                    break;

                case 0b100:
                    ExecuteLoadStoreMultiple(instruction);
                    break;

                case 0b101:
                    ExecuteBranch(instruction);
                    break;

                case 0b110:
                    ExecuteCoprocessorTransfer(instruction);
                    break;

                case 0b111:
                    if (IsBitSet(instruction, 24))
                        ExecuteSoftwareInterrupt(instruction);
                    else
                        ExecuteCoprocessorInstruction(instruction);
                    break;

                default:
                    throw new NotImplementedException($"Unrecognized instruction: 0x{instruction:X8}");
            }
        }
    }
}