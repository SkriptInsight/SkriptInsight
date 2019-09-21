namespace SkriptInsight.Core.Managers.TextDecoration
{
    public class DecorationRenderOptions : ThemableDecorationRenderOptions
    {
        public bool? IsWholeLine { get; set; }

        public ThemableDecorationRenderOptions Light { get; set; }

        public ThemableDecorationRenderOptions Dark { get; set; }
    }
}