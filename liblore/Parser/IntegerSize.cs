using System;

namespace Lore {

    /// <summary>
    /// Integer size.
    /// </summary>
    [Flags]
    public enum IntegerSize {

        // Modifiers
        None,
        Unsigned    = 2 << 0,

        // Signed
        SByte       = 2 << 2,
        SShort      = 2 << 3,
        SWord       = 2 << 4,
        SLong       = 2 << 5,

        // Unsigned
        UByte       = SByte     | Unsigned,
        UShort      = SShort    | Unsigned,
        UWord       = SWord     | Unsigned,
        ULong       = SLong     | Unsigned
    }
}

