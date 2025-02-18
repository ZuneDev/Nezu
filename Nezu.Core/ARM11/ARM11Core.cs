using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nezu.Core.Memory;

using static Nezu.Core.ARM11.RegisterSet;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core : IDisposable
    {
        public RegisterSet Registers = new();
        public RAM Memory = new(512);

        public ARM11Core()
        {
        }

        private void Step() => DecodeAndExecute(Fetch());

        private uint Fetch()
        {
            uint instruction = Memory.ReadWord(Registers[PC]);
            Registers[PC] += sizeof(uint);
            return instruction;
        }


        public void Dispose()
        {
            Memory?.Dispose();
        }
    }
}