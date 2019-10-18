using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Types;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace SkriptInsight.Core.Managers.TextDecoration.ChatColoring
{
    public static class ChatColoringParser
    {
        private static char[] _enumerable;

        private static bool IsColorCode(char ch)
        {
            if (_enumerable == null)
                _enumerable = Enum.GetValues(typeof(ChatColor)).Cast<ChatColor>().Select(c => c.GetChar()).ToArray();
            return _enumerable.Any(c => c == char.ToLower(ch));
        }

        private static List<(char Color, ChatColor Enum)> GetAllColors()
        {
            return Enum.GetValues(typeof(ChatColor)).Cast<ChatColor>().Select(c => (Color: c.GetChar(), Enum: c))
                .ToList();
        }

        private static List<(string[] Alises, ChatColor Enum)> GetAllColorAlises()
        {
            return Enum.GetValues(typeof(ChatColor)).Cast<ChatColor>().Select(c => (Color: c.GetAliases(), Enum: c))
                .ToList();
        }

        public static ChatColoringResult ParseString(string str, long strOffset = 0, long line = 0)
        {
            return new ChatColoringResult
            {
                Snippets = InternalParseString(str, strOffset).ToArray()
            };
        }

        private static IEnumerable<ChatColoredSnippet> InternalParseString(string str, long strOffset = 0,
            long line = 0)
        {
            IEnumerable<ChatColoredSnippet> CloseCurrentColor(StringBuilder stringBuilder, ChatColor chatColor,
                Position position, int index, bool force = false)
            {
                if (force || !(stringBuilder.Length == 0 && chatColor == ChatColor.Reset))
                {
                    var tempStr = stringBuilder.ToString();
                    stringBuilder.Clear();
                    yield return new ChatColoredSnippet
                    {
                        Text = tempStr,
                        Color = chatColor,
                        Range = new Range(position, new Position(line, strOffset + index))
                    };
                }
            }

            var colors = GetAllColors();
            var colorAliases = GetAllColorAlises();
            var buffer = new StringBuilder();
            var startPosition = new Position(line, strOffset);
            var colorBuffer = ChatColor.Reset;

            for (var index = 0; index < str.Length; index++)
            {
                var c = str[index];
                switch (c)
                {
                    case '%':
                        var indexOfUnEscaped = str.GetClosingExpressionIndex(index);
                        if (indexOfUnEscaped > 0)
                        {
                            foreach (var chatColoredSnippet in CloseCurrentColor(buffer, colorBuffer, startPosition,
                                index, true)) yield return chatColoredSnippet;
                            index = indexOfUnEscaped + 1;
                            startPosition = new Position(line, strOffset + index);
                            index--;
                        }

                        break;
                    case '<':
                    case '&':
                    case '§':
                        var isSingleChar = c != '<';
                        var isAliasValid = false;

                        var nextTokenIndex = str.IndexOfUnEscaped('>', index + 1);
                        if (!isSingleChar)
                        {
                            var colorAlias = str.Slice(index + 1, nextTokenIndex);
                            var (aliases, _) =
                                colorAliases.FirstOrDefault(cc => cc.Alises.Any(a => a.Equals(colorAlias)));
                            isAliasValid = aliases != null;
                        }

                        var nextChar = char.ToLower(str.ElementAtOrDefault(index + 1));
                        if (isSingleChar && IsColorCode(nextChar) ||
                            !isSingleChar && nextTokenIndex != -1 && isAliasValid)
                        {
                            foreach (var chatColoredSnippet in CloseCurrentColor(buffer, colorBuffer, startPosition,
                                index)) yield return chatColoredSnippet;

                            ChatColor color;
                            if (isSingleChar)
                            {
                                color = colors.First(cc => cc.Color == nextChar).Enum;
                            }
                            else
                            {
                                var colorAlias = str.Slice(index + 1, nextTokenIndex);
                                var (aliases, chatColor) =
                                    colorAliases.FirstOrDefault(cc => cc.Alises.Any(a => a.Equals(colorAlias)));
                                color = chatColor;
                                if (aliases == null)
                                    color = ChatColor.Reset;
                            }

                            if (colorBuffer.IsReset() || color.IsReset() || !color.IsSpecialFormatting())
                                colorBuffer = color;
                            else
                                colorBuffer |= color;

                            startPosition = new Position(line, strOffset + index);
                            index += 1;
                        }
                        else
                        {
                            buffer.Append(c);
                        }

                        break;

                    default:
                        buffer.Append(c);
                        break;
                }
            }

//            if (buffer.Length > 0)
                yield return new ChatColoredSnippet
                {
                    Text = buffer.ToString(),
                    Color = colorBuffer,
                    Range = new Range(startPosition, new Position(line, strOffset + str.Length))
                };
        }
    }
}