namespace Nezu.Core.Enums
{
    public enum ShiftMode : byte
    {
        LogicalShiftLeft = 0b00,
        LogicalShiftRight = 0b01,
        ArithmeticShiftRight = 0b10,
        RotateRight = 0b11,
    }
}
