using Nezu.Core.Enums;
using static Nezu.Core.Helpers.Math;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        private void DecodeDataProcessing(uint instruction)
        {
            // See A5.1
            bool immediate = IsBitSet(instruction, 25);
            uint opcode = (instruction >> 21) & 0b1111;
            bool s = IsBitSet(instruction, 20);
            uint rn = (instruction >> 16) & 0b1111;
            uint rd = (instruction >> 12) & 0b1111;

            uint shifterOperand;
            var shifterCarryOut = FlagResult.Pass;

            if (immediate)
            {
                uint rotateValue = (instruction >> 8) & 0b1111;
                uint immediateValue = instruction & 0xFF;
                shifterOperand = uint.RotateRight(immediateValue, (int)(rotateValue * 2));
            }
            else
            {
                uint rm = (instruction >> 8) & 0b1111;
                uint rmVal = Registers[rm];

                var shiftMode = (ShiftMode)((instruction >> 4) & 0b11);
                uint shiftAmount = (instruction >> 7) & 0b11111;
                bool isImmediateShift = IsBitSet(instruction, 4);

                if (!isImmediateShift)
                {
                    uint Rs = shiftAmount >> 1;
                    shiftAmount = Registers[Rs] & 0xFF;
                }
                
                if (isImmediateShift && shiftAmount is 0 && shiftMode is ShiftMode.ROR)
                {
                    // See A5.1.13
                    uint C = Registers.IsFlagSet(Flag.C) ? 1u : 0u;
                    shifterOperand = LogicalShiftLeft(C, 31) | LogicalShiftRight(rmVal, 1);
                    shifterCarryOut = (FlagResult)(rmVal & 1);
                }
                else
                {
                    shiftAmount = 32;

                    shifterOperand = Shift(rmVal, (byte)shiftAmount, shiftMode);
                    shifterCarryOut = ShiftCarry(rmVal, (byte)shiftAmount, shiftMode);
                }
            }

            uint rdNew = 0;
            uint rnVal = Registers[rn];

            var carryOut = shifterCarryOut;
            var overflowOut = FlagResult.Pass;

            switch (opcode)
            {
                // AND
                case 0x0:
                    rdNew = rnVal & shifterOperand;
                    break;

                // EOR
                case 0x1:
                    rdNew = rnVal ^ shifterOperand;
                    break;

                // SUB
                case 0x2:
                    shifterOperand = unchecked((uint)(-shifterOperand));
                    rdNew = unchecked(rnVal + shifterOperand);

                    carryOut = CarryFrom(rnVal, shifterOperand, rdNew).ToFlagResult();
                    overflowOut = OverflowFrom(rnVal, shifterOperand, rdNew).ToFlagResult();
                    break;

                // RSB
                case 0x3:
                    uint b = unchecked((uint)(-rnVal));
                    rdNew = unchecked(shifterOperand + b);

                    carryOut = CarryFrom(shifterOperand, b, rdNew).ToFlagResult();
                    overflowOut = OverflowFrom(shifterOperand, b, rdNew).ToFlagResult();
                    break;

                // ADD
                case 0x4:
                    rdNew = unchecked(rnVal + shifterOperand);

                    carryOut = CarryFrom(rnVal, shifterOperand, rdNew).ToFlagResult();
                    overflowOut = OverflowFrom(rnVal, shifterOperand, rdNew).ToFlagResult();
                    break;

                // ADC
                case 0x5:
                    uint carryValue = Registers.IsFlagSet(Flag.C) ? 1u : 0;
                    b = shifterOperand + carryValue;
                    rdNew = unchecked(rnVal + b);

                    carryOut = CarryFrom(rnVal, b, rdNew).ToFlagResult();
                    overflowOut = OverflowFrom(rnVal, b, rdNew).ToFlagResult();
                    break;

                // SBC
                case 0x6:
                    carryValue = Registers.IsFlagSet(Flag.C) ? 0 : 1u;
                    b = unchecked((uint)-(shifterOperand + carryValue));
                    rdNew = unchecked(rnVal + b);

                    carryOut = CarryFrom(rnVal, b, rdNew).ToFlagResult();
                    overflowOut = OverflowFrom(rnVal, b, rdNew).ToFlagResult();
                    break;

                // RSC
                case 0x7:
                    carryValue = Registers.IsFlagSet(Flag.C) ? 0 : 1u;
                    b = unchecked((uint)-(rnVal + carryValue));
                    rdNew = unchecked(shifterOperand + b);

                    carryOut = CarryFrom(shifterOperand, b, rdNew).ToFlagResult();
                    overflowOut = OverflowFrom(shifterOperand, b, rdNew).ToFlagResult();
                    break;

                // TST
                case 0x8:
                    uint aluOut = rnVal & shifterOperand;
                    Registers.ModifyFlag(Flag.N, IsBitSet(aluOut, 31));
                    Registers.ModifyFlag(Flag.Z, aluOut is 0);
                    Registers.ModifyFlag(Flag.C, carryOut);
                    return;

                // TEQ
                case 0x9:
                    aluOut = rnVal ^ shifterOperand;
                    Registers.ModifyFlag(Flag.N, IsBitSet(aluOut, 31));
                    Registers.ModifyFlag(Flag.Z, aluOut is 0);
                    Registers.ModifyFlag(Flag.C, carryOut);
                    return;

                // CMP
                case 0xA:
                    shifterOperand = unchecked((uint)(-shifterOperand));
                    aluOut = unchecked(rnVal + shifterOperand);

                    Registers.ModifyFlag(Flag.N, IsBitSet(aluOut, 31));
                    Registers.ModifyFlag(Flag.Z, aluOut is 0);
                    Registers.ModifyFlag(Flag.C, CarryFrom(rnVal, shifterOperand, rdNew));
                    Registers.ModifyFlag(Flag.V, OverflowFrom(rnVal, shifterOperand, rdNew));
                    return;

                // CMN
                case 0xB:
                    aluOut = unchecked(rnVal + shifterOperand);

                    Registers.ModifyFlag(Flag.N, IsBitSet(aluOut, 31));
                    Registers.ModifyFlag(Flag.Z, aluOut is 0);
                    Registers.ModifyFlag(Flag.C, CarryFrom(rnVal, shifterOperand, rdNew));
                    Registers.ModifyFlag(Flag.V, OverflowFrom(rnVal, shifterOperand, rdNew));
                    return;

                // ORR
                case 0xC:
                    rdNew = rnVal | shifterOperand;
                    break;

                // MOV
                case 0xD:
                    rdNew = shifterOperand;
                    break;

                // MVN
                case 0xF:
                    rdNew = ~shifterOperand;
                    break;

                // BIC
                case 0xE:
                    rdNew = rnVal & ~shifterOperand;
                    break;
            }

            Registers[rd] = rdNew;

            if (s)
            {
                if (rd is RegisterSet.PC)
                {
                    Registers.CPSR = Registers.SPSR;
                }
                else
                {
                    Registers.ModifyFlag(Flag.N, IsBitSet(rdNew, 31));
                    Registers.ModifyFlag(Flag.Z, rdNew is 0);
                    Registers.ModifyFlag(Flag.C, carryOut);
                    Registers.ModifyFlag(Flag.V, overflowOut);
                }
            }
        }
    }
}