using Nezu.Core.ARM11;

namespace Nezu
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RegisterSet reg = new();

            // Start by printing the initial state
            Console.WriteLine("Initial state in User mode:");
            Console.WriteLine($"R13: {reg[13]}");

            // Update to IRQ mode
            reg.UpdateMode(Core.Enums.ARMMode.IRQ);
            Console.WriteLine("\nAfter switching to IRQ mode:");
            Console.WriteLine($"SPSR: {reg.SPSR}");
            Console.WriteLine($"R13: {reg[13]}");

            // Modify R13 in IRQ mode
            reg[13] = 15;
            Console.WriteLine($"\nAfter setting R13 in IRQ mode:");
            Console.WriteLine($"R13: {reg[13]}");

            //Set SPSR in IRQ mode
            reg.SPSR = 200;
            Console.WriteLine($"\nAfter setting SPSR in IRQ mode:");
            Console.WriteLine($"SPSR: {reg.SPSR}");

            // Switch to FIQ mode
            reg.UpdateMode(Core.Enums.ARMMode.FIQ);
            Console.WriteLine("\nAfter switching to FIQ mode:");
            Console.WriteLine($"SPSR: {reg.SPSR}");
            Console.WriteLine($"R13: {reg[13]}");

            // Modify R13 in FIQ mode
            reg[13] = 30;
            Console.WriteLine($"\nAfter setting R13 in FIQ mode:");
            Console.WriteLine($"R13: {reg[13]}");

            // Go back to IRQ mode
            reg.UpdateMode(Core.Enums.ARMMode.IRQ);
            Console.WriteLine("\nAfter switching back to IRQ mode:");
            Console.WriteLine($"SPSR: {reg.SPSR}");
            Console.WriteLine($"R13: {reg[13]}");

            // Final check in User mode
            reg.UpdateMode(Core.Enums.ARMMode.User);
            Console.WriteLine("\nAfter switching back to User mode:");
            //Console.WriteLine($"SPSR: {reg.SPSR}");
            Console.WriteLine($"R13: {reg[13]}");
        }
    }
}
