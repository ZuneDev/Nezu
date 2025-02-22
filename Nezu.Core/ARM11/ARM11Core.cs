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
        private delegate*<ARM11Core, uint, void> DecodeAndExecuteFunc;

        public ARM11Core()
        {
            SetModeARM();
        }

        private void Step() => DecodeAndExecuteFunc(this, FetchFunc(this));

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
            DecodeAndExecuteFunc = &DecodeAndExecuteARMWrapper;
            Registers.ClearFlag(Flag.Thumb);
        }

        private void SetModeThumb()
        {
            FetchFunc = &FetchThumbWrapper;
            DecodeAndExecuteFunc = &DecodeAndExecuteThumbWrapper;
            Registers.SetFlag(Flag.Thumb);
        }


        public void Dispose()
        {
            Memory?.Dispose();
        }
    }
}