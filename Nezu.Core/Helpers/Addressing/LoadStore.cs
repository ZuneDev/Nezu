using System.Runtime.CompilerServices;
using static Nezu.Core.Helpers.Math;

namespace Nezu.Core.ARM11
{
    public unsafe partial class ARM11Core
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint GetBaseLDSTMAddress(uint rn, uint regList, uint instruction)
        {
            uint baseAddr = Memory.ReadWord(Registers[rn]);

            bool writeback = IsBitSet(instruction, 21);
            bool increment = IsBitSet(instruction, 23);
            bool before = IsBitSet(instruction, 24);
            uint steps = SetBitCount(regList) * 4;

            if (increment) baseAddr += (uint)(before ? 4 : 0);
            else baseAddr -= steps + (uint)(before ? 0 : 4);

            if (writeback) Registers[rn] += (uint)(writeback ? steps : -steps);

            return baseAddr;
        }
    }
}