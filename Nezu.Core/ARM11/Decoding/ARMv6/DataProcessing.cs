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
            var shifterCarryOut = CarryResult.Pass;

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
                    shifterCarryOut = (CarryResult)(Rm_val & 1);
                }
                else
                {
                    shiftAmount = 32;

                    shifterOperand = Shift(Rm_val, (byte)shiftAmount, shiftMode);
                    shifterCarryOut = Carry(Rm_val, (byte)shiftAmount, shiftMode);
                }
            }

            switch (opcode)
            {
                // AND
                case 0x0:

                // EOR
                case 0x1:

                // SUB
                case 0x2:

                // RSB
                case 0x3:

                // ADD
                case 0x4:

                // ADC
                case 0x5:

                // SBC
                case 0x6:

                // RSC
                case 0x7:

                // TST
                case 0x8:

                // TEQ
                case 0x9:

                // CMP
                case 0xA:

                // CMN
                case 0xB:

                // ORR
                case 0xC:

                // MOV
                case 0xD:

                // MVN
                case 0xF:
                    throw new NotImplementedException();

                // BIC
                case 0xE:
                    uint newValue = Registers[Rn] & ~shifterOperand;
                    Registers[Rd] = newValue;
                    if (S)
                    {
                        if (Rd is RegisterSet.PC)
                        {
                            Registers.CPSR = Registers.SPSR;
                        }
                        else
                        {
                            Registers.ModifyFlag(Flag.N, IsBitSet(newValue, 31));
                            Registers.ModifyFlag(Flag.Z, newValue is 0);

                            if (shifterCarryOut is not CarryResult.Pass)
                                Registers.ModifyFlag(Flag.C, shifterCarryOut is CarryResult.Set);
                        }
                    }
                    break;
            }
        }
    }
}