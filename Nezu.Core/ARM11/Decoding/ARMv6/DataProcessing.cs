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
            bool S = IsBitSet(instruction, 20);
            uint Rn = (instruction >> 16) & 0b1111;
            uint Rd = (instruction >> 12) & 0b1111;

            uint shifterOperand;
            var shifterCarryOut = FlagResult.Pass;

            // TODO: Set C flag
            if (immediate)
            {
                uint rotateValue = (instruction >> 8) & 0b1111;
                uint immediateValue = instruction & 0xFF;
                shifterOperand = uint.RotateRight(immediateValue, (int)(rotateValue * 2));
            }
            else
            {
                uint Rm = (instruction >> 8) & 0b1111;
                uint Rm_val = Registers[Rm];

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
                    shifterOperand = LogicalShiftLeft(C, 31) | LogicalShiftRight(Rm_val, 1);
                    shifterCarryOut = (FlagResult)(Rm_val & 1);
                }
                else
                {
                    shiftAmount = 32;

                    shifterOperand = Shift(Rm_val, (byte)shiftAmount, shiftMode);
                    shifterCarryOut = ShiftCarry(Rm_val, (byte)shiftAmount, shiftMode);
                }
            }

            uint Rd_new = 0;
            uint Rn_val = Registers[Rn];

            var carryOut = shifterCarryOut;
            var overflowOut = FlagResult.Pass;

            switch (opcode)
            {
                // AND
                case 0x0:
                    Rd_new = Rn_val & shifterOperand;
                    break;

                // EOR
                case 0x1:
                    Rd_new = Rn_val ^ shifterOperand;
                    break;

                // SUB
                case 0x2:
                    shifterOperand = unchecked((uint)(-shifterOperand));
                    Rd_new = unchecked(Rn_val + shifterOperand);

                    carryOut = CarryFrom(Rn_val, shifterOperand, Rd_new).ToFlagResult();
                    overflowOut = OverflowFrom(Rn_val, shifterOperand, Rd_new).ToFlagResult();
                    break;

                // RSB
                case 0x3:
                    uint b = unchecked((uint)(-Rn_val));
                    Rd_new = unchecked(shifterOperand + b);

                    carryOut = CarryFrom(shifterOperand, b, Rd_new).ToFlagResult();
                    overflowOut = OverflowFrom(shifterOperand, b, Rd_new).ToFlagResult();
                    break;

                // ADD
                case 0x4:
                    Rd_new = unchecked(Rn_val + shifterOperand);

                    carryOut = CarryFrom(Rn_val, shifterOperand, Rd_new).ToFlagResult();
                    overflowOut = OverflowFrom(Rn_val, shifterOperand, Rd_new).ToFlagResult();
                    break;

                // ADC
                case 0x5:
                    uint carryValue = Registers.IsFlagSet(Flag.C) ? 1u : 0;
                    b = shifterOperand + carryValue;
                    Rd_new = unchecked(Rn_val + b);

                    carryOut = CarryFrom(Rn_val, b, Rd_new).ToFlagResult();
                    overflowOut = OverflowFrom(Rn_val, b, Rd_new).ToFlagResult();
                    break;

                // SBC
                case 0x6:
                    carryValue = Registers.IsFlagSet(Flag.C) ? 0 : 1u;
                    b = unchecked((uint)-(shifterOperand + carryValue));
                    Rd_new = unchecked(Rn_val + b);

                    carryOut = CarryFrom(Rn_val, b, Rd_new).ToFlagResult();
                    overflowOut = OverflowFrom(Rn_val, b, Rd_new).ToFlagResult();
                    break;

                // RSC
                case 0x7:
                    carryValue = Registers.IsFlagSet(Flag.C) ? 0 : 1u;
                    b = unchecked((uint)-(Rn_val + carryValue));
                    Rd_new = unchecked(shifterOperand + b);

                    carryOut = CarryFrom(shifterOperand, b, Rd_new).ToFlagResult();
                    overflowOut = OverflowFrom(shifterOperand, b, Rd_new).ToFlagResult();
                    break;

                // TST
                case 0x8:
                    uint aluOut = Rn_val & shifterOperand;
                    Registers.ModifyFlag(Flag.N, IsBitSet(aluOut, 31));
                    Registers.ModifyFlag(Flag.Z, aluOut is 0);
                    Registers.ModifyFlag(Flag.C, carryOut);
                    return;

                // TEQ
                case 0x9:
                    aluOut = Rn_val ^ shifterOperand;
                    Registers.ModifyFlag(Flag.N, IsBitSet(aluOut, 31));
                    Registers.ModifyFlag(Flag.Z, aluOut is 0);
                    Registers.ModifyFlag(Flag.C, carryOut);
                    return;

                // CMP
                case 0xA:
                    throw new NotImplementedException();

                // CMN
                case 0xB:
                    throw new NotImplementedException();

                // ORR
                case 0xC:
                    Rd_new = Rn_val | shifterOperand;
                    break;

                // MOV
                case 0xD:
                    Rd_new = shifterOperand;
                    break;

                // MVN
                case 0xF:
                    Rd_new = ~shifterOperand;
                    break;

                // BIC
                case 0xE:
                    Rd_new = Rn_val & ~shifterOperand;
                    break;
            }

            Registers[Rd] = Rd_new;

            if (S)
            {
                if (Rd is RegisterSet.PC)
                {
                    Registers.CPSR = Registers.SPSR;
                }
                else
                {
                    Registers.ModifyFlag(Flag.N, IsBitSet(Rd_new, 31));
                    Registers.ModifyFlag(Flag.Z, Rd_new is 0);
                    Registers.ModifyFlag(Flag.C, carryOut);
                    Registers.ModifyFlag(Flag.V, overflowOut);
                }
            }
        }
    }
}