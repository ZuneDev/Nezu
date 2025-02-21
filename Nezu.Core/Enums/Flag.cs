namespace Nezu.Core.Enums
{
    /// <summary>
    /// The flags used in the status registers.
    /// </summary>
    [Flags]
    public enum Flag : uint
    {
        /// <summary>Disables Fast Interrupt Requests (FIQ)</summary>
        FIQDisable = 1 << 6,

        /// <summary>Disables Interrupt Requests (IRQ)</summary>
        IRQDisable = 1 << 7,

        /// <summary>Disables Abort exceptions</summary>
        AbortDisable = 1 << 8,

        /// <summary>Sets the processor's endianness (BE when 1)</summary>
        Endianness = 1 << 9,

        /// <summary>Indicates Thumb execution mode (when 1)</summary>
        Thumb = 1 << 5,

        /// <summary>Indicates Jazelle execution state (when 1)</summary>
        Jazelle = 1 << 24,

        /// <summary>Indicates saturation occurred (used in SIMD operations)</summary>
        Q = 1 << 27,

        /// <summary>Indicates an overflow condition (signed overflow)</summary>
        V = 1 << 28,

        /// <summary>Indicates a carry or borrow occurred</summary>
        C = 1 << 29,

        /// <summary>Indicates the result was zero</summary>
        Z = 1 << 30,

        /// <summary>Indicates a negative result</summary>
        N = 1u << 31
    }
}