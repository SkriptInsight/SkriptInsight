using SkriptInsight.Core.Types.Attributes;

namespace SkriptInsight.Core.Types
{
    public enum ClickType
    {
        [PatternAlias("left mouse button", "left mouse", "LMB")]
        Left,

        [PatternAlias("left mouse button with shift", "left mouse with shift", "Shift+RMB")]
        ShiftLeft,

        [PatternAlias("right mouse button", "right mouse", "RMB")]
        Right,

        [PatternAlias("right mouse button with shift", "right mouse with shift", "Shift+RMB")]
        ShiftRight,

        [PatternAlias("window border using right mouse button", "window border using right mouse", "border using LMB")]
        WindowBorderLeft,

        [PatternAlias("window border using left mouse button", "window border using right mouse", "border using RMB")]
        WindowBorderRight,

        [PatternAlias("middle mouse button", "middle mouse", "MMB")]
        Middle,
        [PatternAlias("number key", "0-9")] NumberKey,

        [PatternAlias("double click using mouse", "double click")]
        DoubleClick,

        [PatternAlias("drop key", "drop item", "Q")]
        Drop,

        [PatternAlias("drop key with control", "drop stack", "Ctrl+Q")]
        ControlDrop,
        [PatternAlias("creative action")] Creative,

        [PatternAlias("unknown", "unsupported", "custom")]
        Unknown,
    }
}