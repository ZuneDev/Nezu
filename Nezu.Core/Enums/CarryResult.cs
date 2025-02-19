namespace Nezu.Core.Enums
{
    public enum CarryResult : byte
    {
        /// <summary>
        /// Indicates that the carry flag should be reset to 0.
        /// </summary>
        Unset,

        /// <summary>
        /// Indicates that the carry flag should be set to 1.
        /// </summary>
        Set,

        /// <summary>
        /// Indicates that the carry flag should be left unchaged.
        /// </summary>
        Pass
    }
}
