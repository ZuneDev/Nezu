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
            SetModeARM();
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