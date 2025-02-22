using System.Numerics;
using System.Runtime.CompilerServices;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ARM_CLZ(uint instruction) => Registers[(instruction >> 12) & 0xF] = (uint)BitOperations.TrailingZeroCount(Registers[instruction & 0xF]);
    }
}