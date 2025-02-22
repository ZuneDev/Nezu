using Nezu.Core.Enums;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Nezu.Core.Helpers
{
    public static class Math
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(uint value, byte bit) => ((value >> bit) & 1) != 0;

        /// <summary>
        /// Gets the amount of set bits inside of a <see cref="uint"/>.
        /// </summary>
        /// <param name="value">The value to check the population count of.</param>
        /// <returns>The amount of set bits (population count) in the <paramref name="value"/>.</returns>
        /// <remarks>This method relies on the <see cref="BitOperations.PopCount(uint)"/> operation, which is not CLS-compliant.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SetBitCount(uint value) => (uint)BitOperations.PopCount(value);

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
        public static bool CarryFrom(uint a, uint b, uint c) => a > c;

        // Equivalent operation: (IsBitSet(a, 31) && IsBitSet(b, 31)) != IsBitSet(c, 31)
        // By ANDing A and B with 0x80000000, we check if both their 31st bits are set.
        // XORing the result with C's 31st bit tells us if the sign bit of C differs from A and B.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool OverflowFrom(uint a, uint b, uint c) => ((a & b & 0x80000000) ^ (c & 0x80000000)) != 0;


        // The mask is set to 0xFFFFFFFF when the amount is less than 32; otherwise, it is 0.
        // This ensures that any amount over 31 will result in the entire output being zeroed out.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LogicalShiftLeft(uint value, byte amount)
        {
            uint mask = (uint)((amount - 32) >> 31);
            return (value << amount) & mask;
        }

        // Perform an LSL and check for a carry, then, return the flag based on the mask and carry bit.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FlagResult CarryLogicalShiftLeft(uint value, byte amount)
        {
            if (amount == 0)
                return FlagResult.Pass;

            uint mask = (uint)((amount - 32) >> 31);
            byte carryPos = (byte)(32 - amount);
            uint carry = (value >> carryPos) & 1;
            return (FlagResult)((carry & mask) | (uint)(FlagResult.Unset));
        }


        // The mask is set to 0xFFFFFFFF when the amount is less than 32; otherwise, it is 0.
        // This ensures that any amount over 31 will result in the entire output being zeroed out.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LogicalShiftRight(uint value, byte amount)
        {
            uint mask = (uint)((amount - 32) >> 31);
            return (value >> amount) & mask;
        }

        // Perform an LSR and check for a carry, then, return the flag based on the mask and carry bit.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FlagResult CarryLogicalShiftRight(uint value, byte amount)
        {
            if (amount == 0)
                return FlagResult.Pass;

            uint mask = (uint)((amount - 32) >> 31);
            byte carryPos = (byte)((amount - 1) & 31);
            uint carry = (value >> carryPos) & 1;
            return (FlagResult)((carry & mask) | (uint)FlagResult.Unset);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ArithmeticShiftRight(uint value, byte amount)
        {
            // Check MSB to handle shift overflows
            if (amount >= 32) return (value & 0x80000000) != 0 ? 0xFFFFFFFF : 0;
            return (uint)((int)value >> amount);
        }

        // Perform an ASR and check for a carry (or carry overflow), then, return the flag based on the mask and carry bit.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FlagResult CarryArithmeticShiftRight(uint value, byte amount)
        {
            if (amount == 0)
                return FlagResult.Pass;

            uint mask = (uint)((amount - 32) >> 31);
            byte carryPos = (byte)((amount - 1) & 31);
            uint carry = (value >> carryPos) & 1;
            uint largeCarry = (value >> 31) & 1;
            return (FlagResult)((carry & mask) | (largeCarry & ~mask));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateRight(uint value, byte amount) => uint.RotateRight(value, amount);

        // Perform an ROR and check for a carry, then, return the flag based on that carry bit
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FlagResult CarryRotateRight(uint value, byte amount)
        {
            if (amount == 0)
                return FlagResult.Pass;

            byte effectiveAmount = (byte)(amount & 31);
            byte carryBit = (byte)((effectiveAmount - 1) & 31);

            return IsBitSet(value, carryBit).ToFlagResult();
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

        public static FlagResult ShiftCarry(uint value, byte amount, ShiftMode mode)
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