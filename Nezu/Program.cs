using Nezu.Core.ARM11;
using Nezu.Core.Enums;

namespace Nezu
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RegisterSet registers = new RegisterSet();
            Mode[] modes = { Mode.User, Mode.FIQ, Mode.Supervisor, Mode.Abort, Mode.IRQ, Mode.Undefined, Mode.System };

            foreach (var mode in modes)
            {
                registers.SwitchMode(mode);

                for (int i = 0; i < 16; i++)
                    registers[i] = (uint)(i + ((int)mode << 16));

                registers.SPSR = (uint)((int)mode << 24);
            }

            foreach (var mode in modes)
            {
                registers.SwitchMode(mode);
                registers.PrintRegisters();
            }
        }
    }
}
