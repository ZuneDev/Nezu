namespace Nezu.Core.Enums
{
    /// <summary>
    /// The current operating mode of the processor.
    /// </summary>
    public enum Mode : uint
    {
        /// <summary>User mode: PC, R14 to R0, CPSR</summary>
        User = 0b10000,

        /// <summary>FIQ mode: PC, R14_fiq to R8_fiq, R7 to R0, CPSR, SPSR_fiq</summary>
        FIQ = 0b10001,

        /// <summary>IRQ mode: PC, R14_irq, R13_irq, R12 to R0, CPSR, SPSR_irq</summary>
        IRQ = 0b10010,

        /// <summary>Supervisor mode: PC, R14_svc, R13_svc, R12 to R0, CPSR, SPSR_svc</summary>
        Supervisor = 0b10011,

        /// <summary>Abort mode: PC, R14_abt, R13_abt, R12 to R0, CPSR, SPSR_abt</summary>
        Abort = 0b10111,

        /// <summary>Undefined mode: PC, R14_und, R13_und, R12 to R0, CPSR, SPSR_und</summary>
        Undefined = 0b11011,

        /// <summary>System mode: PC, R14 to R0, CPSR (ARMv4 and above)</summary>
        System = 0b11111
    }
}