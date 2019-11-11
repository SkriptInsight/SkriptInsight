using System.Linq;
using SkriptInsight.Core.Extensions;
using SkriptInsight.Core.Managers;
using SkriptInsight.Core.Parser;
using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.Parser.Functions;
using SkriptInsight.Core.Parser.Types.Impl.Internal;
using SkriptInsight.Core.Utils;
using Xunit;

namespace SkriptInsight.Tests
{
    public class SkriptFunctionParserTests
    {
        [Theory]
        [InlineData("test", "string")]
        [InlineData("aaaa", "number")]
        [InlineData("bla", "long")]
        [InlineData("aa:aa", "number")]
        public void FunctionParameterParserWorks(string expectedName, string expectedType)
        {
            var param = new SkriptFunctionParameter();
            var input = $"{expectedName}: {expectedType}";
            var result = param.TryParseValue(input);

            Assert.NotNull(result);
            var expr = result.GetExplicitValue<FunctionParameter>(0)?.GenericValue;

            Assert.NotNull(expr);

            Assert.Equal(expectedName, expr.Name);
            Assert.Equal(expectedType, expr.Type.TypeName);
            Assert.Equal(input, expr.ToString());
        }

        [Theory]
        [InlineData("function mcrremoveColor(msg: text) :: text:")]
        [InlineData("function mcrjsonColorize(msg: text, default-color: text = \" & r\") :: text:")]
        [InlineData("function mcrjsonSanitize(msg: text) :: text:")]
        [InlineData("function mcrjsonFormat(msg: text, color: boolean = true) :: text:")]
        [InlineData("function mcrjson(to: text, msg: text, color: boolean = true):")]
        [InlineData("function mcrjsonBroadcast(msg: text, color: boolean = true):")]
        [InlineData("function parseRank(p: player , r: text) :: string:")]
        [InlineData("function parseRankRaw(r: text) :: string:")]
        [InlineData("function playerdata(p: offline player) :: boolean:")]
        [InlineData("function getPlayerdata(p: offline player , f: text) :: string:")]
        [InlineData("function setPlayerdata(p: offline player , f: text , v: text) :: boolean:")]
        [InlineData("function addShards(p: offline player , a: number) :: boolean:")]
        [InlineData("function delShards(p: offline player , a: number) :: boolean:")]
        [InlineData("function addGems(p: offline player , a: number) :: boolean:")]
        [InlineData("function delGems(p: offline player , a: number) :: boolean:")]
        [InlineData(
            "function firework(l: location, t: string, r: integer, g: integer, b: integer, p: boolean) :: string:")]
        [InlineData("function Beam(e1: entity , e2: entity):")]
        [InlineData("function ClientBeam(e1: entity , e3: entity , e2: entity):")]
        [InlineData("function configRefresh(n: number) :: number:")]
        [InlineData("function configPopulate(n: number) :: number:")]
        [InlineData("function updateRankAPI(p: player) :: player:")]
        [InlineData("function mcs(p: player , m: text) :: number:")]
        [InlineData("function caseSensitive(source: String, compareTo: String) :: boolean:")]
        [InlineData("function gwenAlert(p: player , r: text) :: number:")]
        [InlineData("function carlSpinNew(p: player) :: text:")]
        [InlineData("function reqSpin(p: player) :: text:")]
        [InlineData("function carlSpin(p: player) :: number:")]
        [InlineData("function ytlink(p: player) :: number:")]
        [InlineData("function tlink(p: player) :: number:")]
        [InlineData("function dailyr(p: player) :: number:")]
        [InlineData("function rawVote(p: player , v: number) :: number:")]
        [InlineData("function pollVote(p: player) :: number:")]
        [InlineData("function claimThank(p: player) :: number:")]
        [InlineData("function carlGUI(p: player) :: number:")]
        [InlineData("function createTitle(arg1: player , arg2: text) :: number:")]
        [InlineData("function removeTitle(arg1: player) :: number:")]
        [InlineData("function trackDB(p: player , t: string) :: string:")]
        [InlineData("function mountapi(p: player, action: text, type: text) :: number:")]
        [InlineData("function cosmetic_system(arg1: text , arg2: text , p: player) :: number:")]
        [InlineData("function resetPoints(p: offline player, l: number) :: number:")]
        [InlineData("function rewardTreasure(p: player , c: number , l: number , s: number) :: number:")]
        [InlineData("function buyChest(p: player, n: number) :: player:")]
        [InlineData("function openTreasure(p: player , c: number , l: number) :: number:")]
        [InlineData("function TreasurePage(p: player , n: number , c: number) :: player:")]
        [InlineData("function animSB(p: player) :: text:")]
        [InlineData("function rankLoad(p: player , n: number , t: number) :: string:")]
        [InlineData("function runNews(p: player) :: boolean:")]
        [InlineData("function newsinit(i: integer) :: number:")]
        [InlineData("function friendButtons(p: player) :: number:")]
        [InlineData("function friendClean(p: player) :: number:")]
        [InlineData("function friendMain(p: player) :: number:")]
        [InlineData("function friendDel(p: player) :: number:")]
        [InlineData("function friendReq(p: player) :: number:")]
        [InlineData(
            "function rawreport(arg1: player , arg2: player , arg3: text , arg4: text , arg5: text) :: string:")]
        [InlineData("function punishWarn(s: player , p: offline player , r: text) :: player:")]
        [InlineData("function punishReportBan(s: player , p: offline player , r: text) :: player:")]
        [InlineData("function punishBan(s: player , p: offline player , r: text) :: player:")]
        [InlineData("function punishDTempBan(s: player , p: offline player, r: text, d: number) :: player:")]
        [InlineData("function punishHTempBan(s: player , p: offline player, r: text, d: number) :: player:")]
        [InlineData("function punishMute(s: player, p: offline player, r: text) :: player:")]
        [InlineData("function punishHTempMute(s: player, p: offline player, r: text, d: number) :: player:")]
        [InlineData("function punishDTempMute(s: player, p: offline player, r: text, d: number) :: player:")]
        [InlineData("function punishUnban(s: string, p: string) :: player:")]
        [InlineData("function punishUnmute(s: player, p: string) :: player:")]
        [InlineData("function prefsSystem(p: player , t: number) :: number:")]
        [InlineData("function profile_system(arg1: text , arg2: text , p: player, p2: player) :: number:")]
        [InlineData("function beamGo(p: player) :: number:")]
        public void FunctionSignatureKnowsHowToParseSignatures(string signature)
        {
            var ctx = ParseContext.FromCode(signature);

            var sign = FunctionSignature.TryParse(ctx);

            Assert.NotNull(sign);
        }

        [Fact]
        public void FunctionSignatureKnowsHowToParseRetardedSignatures()
        {
            //function retarded_func(b: strings = "thing","", c: integer) :: string
            var ctx = ParseContext.FromCode(
                "function retarded_func(b: strings = \"thing\",\"\", c: integer) :: string");

            var signature = FunctionSignature.TryParse(ctx);

            Assert.NotNull(signature);
            Assert.Equal(2, signature.Parameters.Count);

            var firstParam = signature.Parameters[0];
            Assert.Equal("b", firstParam.Name);
            Assert.Equal("strings", firstParam.Type.FinalTypeName);
            Assert.Equal("thing", firstParam.DefaultValue.GetExplicitValue<string>(0).GenericValue);
            Assert.Equal(string.Empty, firstParam.DefaultValue.GetExplicitValue<string>(1).GenericValue);


            var secondParam = signature.Parameters[1];
            Assert.Equal("c", secondParam.Name);
            Assert.Equal("integer", secondParam.Type.FinalTypeName);
        }

        [Theory]
        [InlineData("test", "", "test: string", "testing2: number")]
        [InlineData("test", "string", "test: string", "testing2: number")]
        public void FunctionSignatureParserWorks(string name, string returnType = "", params string[] input)
        {
            var _ = NodeSignaturesManager.Instance;
            var result = $"function {name}({string.Join(", ", input)}){(returnType != "" ? $" :: {returnType}" : "")}";

            var ctx = ParseContext.FromCode(result);
            var signature = SignatureParserHelper.TryParse<FunctionSignature>(ctx);

            Assert.NotNull(signature);

            Assert.Equal(result, signature.ToString());

            Assert.Equal(name, signature.Name);

            if (!returnType.IsEmpty()) Assert.Equal(returnType, signature.ReturnType.FinalTypeName);

            Assert.Equal(input, signature.Parameters.Select(p => p.ToString()));

            Assert.True(ctx.HasFinishedLine, "ctx.HasFinishedLine");
        }
    }
}