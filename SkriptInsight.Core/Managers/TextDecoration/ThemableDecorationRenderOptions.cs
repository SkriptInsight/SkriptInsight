using JetBrains.Annotations;

namespace SkriptInsight.Core.Managers.TextDecoration
{
    public class ThemableDecorationRenderOptions
    {
        [CanBeNull] public string BackgroundColor { get; set; }

        [CanBeNull] public string Outline { get; set; }

        [CanBeNull] public string OutlineColor { get; set; }

        [CanBeNull] public string OutlineStyle { get; set; }

        [CanBeNull] public string OutlineWidth { get; set; }

        [CanBeNull] public string Border { get; set; }

        [CanBeNull] public string BorderColor { get; set; }

        [CanBeNull] public string BorderRadius { get; set; }

        [CanBeNull] public string BorderSpacing { get; set; }

        [CanBeNull] public string BorderStyle { get; set; }

        [CanBeNull] public string BorderWidth { get; set; }

        [CanBeNull] public string FontWeight { get; set; }

        [CanBeNull] public string TextDecoration { get; set; }

        [CanBeNull] public string Cursor { get; set; }

        [CanBeNull] public string Color { get; set; }

        [CanBeNull] public string Opacity { get; set; }

        [CanBeNull] public string LetterSpacing { get; set; }
    }
}