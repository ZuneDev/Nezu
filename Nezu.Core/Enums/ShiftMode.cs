namespace Nezu.Core.Enums
{
    public enum ShiftMode : byte
    {
        /// <summary>
        /// Logical shift left
        /// </summary>
        LSL = 0b00,

        /// <summary>
        /// Logical shift right
        /// </summary>
        LSR = 0b01,

        /// <summary>
        /// Arithmetic shift right
        /// </summary>
        ASR = 0b10,

        /// <summary>
        /// Rotate right
        /// </summary>
        /// <remarks>
        /// If immediate shift amount is 0, "rotate right with extend" (33-bit rotate via Carry flag)
        /// </remarks>
        ROR = 0b11,
    }
}
