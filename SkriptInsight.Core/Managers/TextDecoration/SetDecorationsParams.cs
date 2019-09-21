using System;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Core.Managers.TextDecoration
{
    public class SetDecorationsParams
    {
        public SetDecorationsParams(Uri uri, TextEditorDecorationType decorationType, IEnumerable<Range> ranges)
        {
            DecorationType = decorationType;
            Uri = uri;
            Ranges = new Container<Range>(ranges);
        }

        public Uri Uri { get; }

        public TextEditorDecorationType DecorationType { get; }

        public Container<Range> Ranges { get; }
    }
}