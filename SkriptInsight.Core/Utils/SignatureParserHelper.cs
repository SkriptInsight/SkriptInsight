using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SkriptInsight.Core.Parser;

namespace SkriptInsight.Core.Utils
{
    public static class SignatureParserHelper
    {
        private static Dictionary<Type, Delegate> SignatureDelegates { get; } = new Dictionary<Type, Delegate>();

        [CanBeNull]
        [PublicAPI]
        public static object TryParse(Type t, ParseContext ctx)
        {
            var del = GetTryParseDelegateForType(t);
            return del.DynamicInvoke(ctx);
        }
        
        [CanBeNull]
        public static T TryParse<T>(ParseContext ctx)
        {
            return (T) TryParse(typeof(T), ctx);
        }

        public static Delegate GetTryParseDelegateForType(Type t)
        {
            const string tryParseMethod = "TryParse";

            if (SignatureDelegates.ContainsKey(t))
                return SignatureDelegates[t];

            var del = Delegate.CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(ParseContext), t), t,
                tryParseMethod);

            SignatureDelegates[t] = del;
            return del;
        }
    }
}