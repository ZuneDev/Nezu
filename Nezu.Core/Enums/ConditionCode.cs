namespace Nezu.Core.Enums
{
    [Flags]
    public enum ConditionCode : uint
    {
        /// <summary>Equal (EQ): Z = 1</summary>
        EQ = 0b0000,

        /// <summary>Not Equal (NE): Z = 0</summary>
        NE = 0b0001,

        /// <summary>C Set (CS): C = 1</summary>
        CS = 0b0010,

        /// <summary>C Clear (CC): C = 0</summary>
        CC = 0b0011,

        /// <summary>Minus (MI): N = 1</summary>
        MI = 0b0100,

        /// <summary>Plus (PL): N = 0</summary>
        PL = 0b0101,

        /// <summary>V (VS): V = 1</summary>
        VS = 0b0110,

        /// <summary>No V (VC): V = 0</summary>
        VC = 0b0111,

        /// <summary>Unsigned Higher (HI): C = 1 and Z = 0</summary>
        HI = 0b1000,

        /// <summary>Unsigned Lower or Same (LS): C = 0 or Z = 1</summary>
        LS = 0b1001,

        /// <summary>Signed Greater than or Equal (GE): N = V</summary>
        GE = 0b1010,

        /// <summary>Signed Less than (LT): N != V</summary>
        LT = 0b1011,

        /// <summary>Signed Greater than (GT): Z = 0 and N = V</summary>
        GT = 0b1100,

        /// <summary>Signed Less than or Equal (LE): Z = 1 or N != V</summary>
        LE = 0b1101,

        /// <summary>Always (AL): No condition, always true</summary>
        AL = 0b1110,

        /// <summary>Undefined (UD): Reserved condition code (invalid)</summary>
        UD = 0b1111
    }
}