using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Files;
using SkriptInsight.Core.Managers.TextDecoration;
using SkriptInsight.Core.Managers.TextDecoration.ChatColoring;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Expressions.Variables;
using SkriptInsight.Core.Types;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Core.Managers
{
    public class MinecraftColoringManager
    {
        private static MinecraftColoringManager _instance;

        private readonly Action<object, EventArgs> _action;
        private readonly Action<object, EventArgs> _actionDebounced;

        private MinecraftColoringManager()
        {
            _action = File_OnParseCompleteDebounced;
            _actionDebounced = _action/*.Debounce(450)*/;
        }

        public ConcurrentDictionary<ChatColor, TextEditorDecorationType> ChatColorsDecoration { get; } =
            new ConcurrentDictionary<ChatColor, TextEditorDecorationType>();

        public ISkriptInsightHost Host => WorkspaceManager.CurrentHost;

        public static MinecraftColoringManager Instance => _instance ??= new MinecraftColoringManager();

        public TextEditorDecorationType GetDecorationTypeFor(ChatColor color)
        {
            if (ChatColorsDecoration.ContainsKey(color))
                return ChatColorsDecoration[color];

            var options = new DecorationRenderOptions();

            var colors = color.GetColors();

            foreach (var chatColor in colors)
            {
                var optionsColor = chatColor.GetColorRgb();
                var optionsFontWeight = chatColor.GetFontWeight();
                var optionsTextDecoration = chatColor.GetTextDecoration();

                if (optionsColor != "")
                    options.Color = optionsColor;

                if (optionsFontWeight != "")
                    options.FontWeight = optionsFontWeight;

                if (optionsTextDecoration != "")
                    options.TextDecoration = optionsTextDecoration;
            }

            var decorationType = Host?.CreateTextEditorDecorationType(options).Result;

            ChatColorsDecoration[color] = decorationType;

            return decorationType;
        }

        public void HandleFile(SkriptFile file)
        {
//            file.GotFocus += File_OnParseComplete;
//            file.SyntaxParsed += File_OnParseComplete;
//            file.Closed += (s, e) => file.SyntaxParsed -= File_OnParseCompleteDebounced;
        }

        public void File_OnParseComplete(object sender, EventArgs e)
        {
            _actionDebounced?.Invoke(sender, e);
        }

        private void File_OnParseCompleteDebounced(object sender, EventArgs e)
        {
            var decorations = new List<(TextEditorDecorationType, Range)>();
            if (!(sender is SkriptFile file)) return;

/*
            if (false)
            {
                if (ChatColorsDecoration.Count <= 0) return;
                //Clear existing chat coloring decorations
                foreach (var textEditorDecorationType in ChatColorsDecoration)
                    LanguageServer.SetDecorations(file.Url, textEditorDecorationType.Value, new Range[0]);

                foreach (var textEditorDecorationType in ChatColorsDecoration)
                    textEditorDecorationType.Value?.Dispose();

                ChatColorsDecoration.Clear();

                return;
            }
*/

            var nodes = file.Nodes;

            foreach (var node in nodes.Select(c => c.Value))
                if (node?.MatchedSyntax?.Result?.IsSuccess ?? false)
                {
                    foreach (var value in node.MatchedSyntax?.Result?.Matches.OfType<ExpressionParseMatch>()
                        .SelectMany(r => r.Expression.GetValues<string>()))
                        if (value is Expression<string> stringValue && stringValue.Range != null)
                        {
                            var result = ChatColoringParser.ParseString(stringValue.GenericValue,
                                value.Range.Start.Character, value.Range.Start.Line);

                            decorations.AddRange(result.Snippets.Select(coloredSnippet =>
                                (GetDecorationTypeFor(coloredSnippet.Color),
                                    new Range(
                                        new Position(node.LineNumber, coloredSnippet.Range.Start.Character + 1),
                                        new Position(node.LineNumber, coloredSnippet.Range.End.Character + 1)))));
                        }
                }


            //Clear existing chat coloring decorations
            foreach (var textEditorDecorationType in ChatColorsDecoration)
                Host.SetDecorations(file.Url, textEditorDecorationType.Value, new Range[0]);

            //Send coloring
            foreach (var valueTuples in decorations.GroupBy(c => c.Item1))
                Host.SetDecorations(file.Url, valueTuples.Key, valueTuples.Select(c => c.Item2));
        }

    }
}