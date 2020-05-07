using System;

namespace SkriptInsight.JavaMetadata.Model
{
    [Flags]
    public enum JavaAccess
    {
        None = 0,
        Public = 0x0001,
        Private = 0x0002,
        Protected = 0x0004,
        Static = 0x0008,
        Final = 0x0010,
        Open = 0x0020,
        Super = 0x0020,
        Synchronized = 0x0020,
        Transitive = 0x0020,
        Bridge = 0x0040,
        StaticPhase = 0x0040,
        Volatile = 0x0040,
        Transient = 0x0080,
        Varargs = 0x0080,
        Native = 0x0100,
        Interface = 0x0200,
        Abstract = 0x0400,
        Strict = 0x0800,
        Synthetic = 0x1000,
        Annotation = 0x2000,
        Enum = 0x4000,
        Mandated = unchecked((short) 0x8000),
        Module = unchecked((short) 0x8000),

    }
}