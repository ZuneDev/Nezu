using System.Runtime.CompilerServices;
using Nezu.Core.Enums;

namespace Nezu.Core.Helpers
{
    public static class Math
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(uint value, byte bit) => (value & 1 << bit) != 0;

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

        public static uint LogicalShiftLeftWithCarry(uint value, byte amount, out CarryResult carry)
        {
            if (amount == 0)
            {
                carry = CarryResult.Unset;
                return value;
            }
            else if (amount < 32)
            {
                carry = IsBitSet(value, (byte)(32 - amount))
                    ? CarryResult.Set : CarryResult.Unset;
                return value << amount;
            }
            else if (amount == 32)
            {
                carry = (CarryResult)(value & 1);
                return 0;
            }
            else
            {
                carry = CarryResult.Unset;
                return 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LogicalShiftLeft(uint value, byte amount)
        {
            return amount switch
            {
                0 => value,
                < 32 => value << amount,
                _ => 0
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CarryResult LogicalShiftLeftCarry(uint value, byte amount)
        {
            return amount switch
            {
                0 => CarryResult.Pass,
                < 32 => IsBitSet(value, (byte)(32 - amount))
                    ? CarryResult.Set : CarryResult.Unset,
                _ => CarryResult.Unset
            };
        }
    }
}