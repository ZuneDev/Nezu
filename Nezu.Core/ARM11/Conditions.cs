using Nezu.Core.Enums;
using System.Runtime.CompilerServices;

namespace Nezu.Core.ARM11
{
    public partial class ARM11Core
    {
        private bool IsConditionSet(ConditionCode condition) => condition switch
        {
            ConditionCode.EQ => Registers.IsFlagSet(ARMFlag.Z),
            ConditionCode.NE => !Registers.IsFlagSet(ARMFlag.Z),
            ConditionCode.CS => Registers.IsFlagSet(ARMFlag.C),
            ConditionCode.CC => !Registers.IsFlagSet(ARMFlag.C),
            ConditionCode.MI => Registers.IsFlagSet(ARMFlag.N),
            ConditionCode.PL => !Registers.IsFlagSet(ARMFlag.N),
            ConditionCode.VS => Registers.IsFlagSet(ARMFlag.V),
            ConditionCode.VC => !Registers.IsFlagSet(ARMFlag.V),
            ConditionCode.HI => IsConditionHI(),
            ConditionCode.LS => IsConditionLS(),
            ConditionCode.GE => IsConditionGE(),
            ConditionCode.LT => IsConditionLT(),
            ConditionCode.GT => !Registers.IsFlagSet(ARMFlag.Z) && IsConditionGE(),
            ConditionCode.LE => Registers.IsFlagSet(ARMFlag.Z) || IsConditionLT(),
            ConditionCode.AL => true,
            ConditionCode.UD => false,
            _ => false
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsConditionHI() => Registers.IsFlagSet(ARMFlag.C) && !Registers.IsFlagSet(ARMFlag.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsConditionLS() => !Registers.IsFlagSet(ARMFlag.C) || Registers.IsFlagSet(ARMFlag.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsConditionGE() => Registers.IsFlagSet(ARMFlag.N) == Registers.IsFlagSet(ARMFlag.V);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsConditionLT() => Registers.IsFlagSet(ARMFlag.N) != Registers.IsFlagSet(ARMFlag.V);
    }
}