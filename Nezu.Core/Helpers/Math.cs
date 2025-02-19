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
        public static CarryResult CarryLogicalShiftLeft(uint value, byte amount)
        {
            return amount switch
            {
                0 => CarryResult.Pass,
                < 32 => IsBitSet(value, (byte)(32 - amount))
                    ? CarryResult.Set : CarryResult.Unset,
                _ => CarryResult.Unset
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LogicalShiftRight(uint value, byte amount)
        {
            // See A5.1.8
            return amount switch
            {
                0 => value,
                < 32 => value >> amount,
                _ => 0
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CarryResult CarryLogicalShiftRight(uint value, byte amount)
        {
            return amount switch
            {
                0 => CarryResult.Pass,
                < 32 => IsBitSet(value, (byte)(amount - 1))
                    ? CarryResult.Set : CarryResult.Unset,
                _ => CarryResult.Unset
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ArithmeticShiftRight(uint value, byte amount)
        {
            // See A5.1.10
            return amount switch
            {
                0 => value,
                < 32 => unchecked((uint)((int)value >> amount)),
                _ => unchecked((int)value) >= 0
                    ? 0
                    : uint.MaxValue
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CarryResult CarryArithmeticShiftRight(uint value, byte amount)
        {
            return amount switch
            {
                0 => CarryResult.Pass,
                < 32 => IsBitSet(value, (byte)(amount - 1))
                    ? CarryResult.Set : CarryResult.Unset,
                _ => IsBitSet(value, 31)
                    ? CarryResult.Set : CarryResult.Unset,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateRight(uint value, byte amount)
        {
            // See A5.1.12
            return uint.RotateRight(value, amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CarryResult CarryRotateRight(uint value, byte amount)
        {
            if (amount == 0)
                return CarryResult.Pass;

            byte effectiveAmount = (byte)(amount & 0b11111);
            byte carryBit = (byte)(effectiveAmount == 0
                ? 31
                : effectiveAmount - 1);

            return IsBitSet(value, carryBit)
                ? CarryResult.Set : CarryResult.Unset;
        }

        public static uint Shift(uint value, byte amount, ShiftMode mode)
        {
            return mode switch
            {
                ShiftMode.LSL => LogicalShiftLeft(value, amount),
                ShiftMode.LSR => LogicalShiftRight(value, amount),
                ShiftMode.ASR => ArithmeticShiftRight(value, amount),
                ShiftMode.ROR => RotateRight(value, amount),
                _ => throw new NotImplementedException()
            };
        }

        public static CarryResult Carry(uint value, byte amount, ShiftMode mode)
        {
            return mode switch
            {
                ShiftMode.LSL => CarryLogicalShiftLeft(value, amount),
                ShiftMode.LSR => CarryLogicalShiftRight(value, amount),
                ShiftMode.ASR => CarryArithmeticShiftRight(value, amount),
                ShiftMode.ROR => CarryRotateRight(value, amount),
                _ => throw new NotImplementedException()
            };
        }
    }
}