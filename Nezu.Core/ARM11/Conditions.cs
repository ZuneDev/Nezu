using Nezu.Core.Enums;
using System.Runtime.CompilerServices;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        private bool IsConditionSet(ConditionCode condition) => condition switch
        {
            ConditionCode.EQ => Registers.IsFlagSet(Flag.Z),
            ConditionCode.NE => !Registers.IsFlagSet(Flag.Z),
            ConditionCode.CS => Registers.IsFlagSet(Flag.C),
            ConditionCode.CC => !Registers.IsFlagSet(Flag.C),
            ConditionCode.MI => Registers.IsFlagSet(Flag.N),
            ConditionCode.PL => !Registers.IsFlagSet(Flag.N),
            ConditionCode.VS => Registers.IsFlagSet(Flag.V),
            ConditionCode.VC => !Registers.IsFlagSet(Flag.V),
            ConditionCode.HI => IsConditionHI(),
            ConditionCode.LS => IsConditionLS(),
            ConditionCode.GE => IsConditionGE(),
            ConditionCode.LT => IsConditionLT(),
            ConditionCode.GT => !Registers.IsFlagSet(Flag.Z) && IsConditionGE(),
            ConditionCode.LE => Registers.IsFlagSet(Flag.Z) || IsConditionLT(),
            ConditionCode.AL => true,
            ConditionCode.UD => true,
            _ => false
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsConditionHI() => Registers.IsFlagSet(Flag.C) && !Registers.IsFlagSet(Flag.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsConditionLS() => !Registers.IsFlagSet(Flag.C) || Registers.IsFlagSet(Flag.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsConditionGE() => Registers.IsFlagSet(Flag.N) == Registers.IsFlagSet(Flag.V);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsConditionLT() => Registers.IsFlagSet(Flag.N) != Registers.IsFlagSet(Flag.V);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsBitSet(uint value, byte bit) => (value & (1 << bit)) != 0;
    }
}