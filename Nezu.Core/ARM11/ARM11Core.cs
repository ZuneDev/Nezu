using Nezu.Core.Enums;
using Nezu.Core.Memory;

using static Nezu.Core.ARM11.RegisterSet;

namespace Nezu.Core.ARM11
{
    public unsafe partial class ARM11Core : IDisposable
    {
        public RegisterSet Registers = new();
        public RAM Memory = new(512);

        private delegate*<ARM11Core, uint> FetchFunc;
        private delegate*<ARM11Core, uint, void> DecodeExecFunc;

        public ARM11Core()
        {
            Memory.WriteWord(0x00, 0xE3A0000A);
            Memory.WriteWord(0x04, 0xE3A01003);
            Memory.WriteWord(0x08, 0xEBFFFFFE);
            Memory.WriteWord(0x0C, 0xE0800001);
            Memory.WriteWord(0x10, 0xE12FFF1E);
            SetModeARM();
            Step();
            Registers.PrintRegisters();
            Step();
            Registers.PrintRegisters();
            Step();
            Registers.PrintRegisters();
            Step();
            Registers.PrintRegisters();
            Step();
            Registers.PrintRegisters();
            Step();
        }

        private void Step() => DecodeExecFunc(this, FetchFunc(this));

        private static uint FetchARMWrapper(ARM11Core core) => core.FetchARM();
        private static uint FetchThumbWrapper(ARM11Core core) => core.FetchThumb();
        private static void DecodeAndExecuteARMWrapper(ARM11Core core, uint instr) => core.DecodeAndExecuteARM(instr);
        private static void DecodeAndExecuteThumbWrapper(ARM11Core core, uint instr) => core.DecodeAndExecuteThumb(instr);

        private uint FetchARM()
        {
            uint instruction = Memory.ReadWord(Registers[PC]);
            Registers[PC] += sizeof(uint);
            return instruction;
        }

        private uint FetchThumb()
        {
            uint instruction = Memory.ReadHalfWord(Registers[PC]);
            Registers[PC] += sizeof(ushort);
            return instruction;
        }


        private void SetModeARM()
        {
            FetchFunc = &FetchARMWrapper;
            DecodeExecFunc = &DecodeAndExecuteARMWrapper;
            Registers.ClearFlag(Flag.Thumb);
        }

        private void SetModeThumb()
        {
            FetchFunc = &FetchThumbWrapper;
            DecodeExecFunc = &DecodeAndExecuteThumbWrapper;
            Registers.SetFlag(Flag.Thumb);
        }


        public void Dispose()
        {
            Memory?.Dispose();
        }
    }
}