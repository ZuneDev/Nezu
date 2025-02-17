using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core : IDisposable
    {
        public RegisterSet Registers = new();
        public Memory.Memory Memory = new(512);

        public ARM11Core()
        {
        }

        private void Step()
        {
            // Fetch
            var instruction = Memory.ReadWord(Registers[15]);

            // Decode & Execute
            ExecuteInstruction(instruction);

            // Increment PC
            ++Registers[15];
        }

        public void Dispose()
        {
            Memory?.Dispose();
        }
    }
}