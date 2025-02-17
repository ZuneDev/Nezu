using System;
using System.Runtime.CompilerServices;

namespace Nezu.Core
{
    public static class Math
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(uint value, byte bit) => (value & (1 << bit)) != 0;

        /// <summary>
        /// Promotes a signed 24-bit value to a signed 32-bit value.
        /// </summary>
        /// <param name="value24">The raw 24-bit value.</param>
        /// <returns>An <see cref="int"/> with an equivalent numerical value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ExpandToInt32(uint value24)
        {
            return IsBitSet(value24, 23)
                ? unchecked((int)(value24 | 0xFF000000))
                : (int)value24;
        }
    }
}
