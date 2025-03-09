using Nezu.Core.Enums;
using Nezu.Core.Memory;

using static Nezu.Core.ARM11.RegisterSet;

namespace Nezu.Core.ARM11
{
    public unsafe partial class ARM11Core : IDisposable
    {
        public RegisterSet Registers = new();
        public RAM Memory = new(512);

        private uint _instruction;
        private uint _nextInstr;

        private delegate*<ARM11Core, uint> FetchFunc;
        private delegate*<ARM11Core, uint, void> DecodeExecFunc;

        public ARM11Core()
        {
            Memory.WriteWord(0x00, 0xE1A00000);
            Memory.WriteWord(0x04, 0xEB000003);
            Memory.WriteWord(0x08, 0xE1A01000);
            Memory.WriteWord(0x0C, 0xE3A00000);
            Memory.WriteWord(0x10, 0xEB000000);
            Memory.WriteWord(0x14, 0xEAFFFFFE);
            Memory.WriteWord(0x18, 0xE3A0000A);
            Memory.WriteWord(0x1C, 0xE12FFF1E);
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
            Registers.PrintRegisters();
        }

        public void Step() => DecodeExecFunc(this, FetchFunc(this));

        public void Reset()
        {
            Registers.ResetRegisters();
            Registers.CPSR = 0xD3;
            Registers.SwitchMode(Mode.Supervisor);
            Registers[PC] = 0; // TODO: handle high vectors; relies on CP15.

            _instruction = 0;
            SetModeARM();
        }

        #region Fetch/Decode mode handlers
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
        #endregion


        public void Dispose()
        {
            Memory?.Dispose();
        }
    }
}