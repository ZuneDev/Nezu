using System.Runtime.CompilerServices;

namespace Nezu.Core.Enums
{
    public enum FlagResult : byte
    {
        /// <summary>
        /// Indicates that the flag should be reset to 0.
        /// </summary>
        Unset,

        /// <summary>
        /// Indicates that the flag should be set to 1.
        /// </summary>
        Set,

        /// <summary>
        /// Indicates that the flag should be left unchaged.
        /// </summary>
        Pass
    }

    public static class FlagResultExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ToBool(this FlagResult result) => result is FlagResult.Set;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FlagResult ToFlagResult(this bool set) => set ? FlagResult.Set : FlagResult.Unset;
    }
}