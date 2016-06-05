using System;

namespace Lore {

    /// <summary>
    /// Special value.
    /// </summary>
    public enum SpecialValue {
        None = 0,
        String = 1 << 0,
        Enumeration = 1 << 2,
        Structure = 1 << 3,
        Boolean = 1 << 4,
    }
}

