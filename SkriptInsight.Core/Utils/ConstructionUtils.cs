using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SkriptInsight.Core.Utils
{
    public static class ConstructionUtils
    {
        private static Dictionary<Type, Delegate> ConstructorDelegateCache { get; } = new Dictionary<Type, Delegate>();

        public static object NewInstance(this Type t, params object[] args)
        {
            if (ConstructorDelegateCache.ContainsKey(t)) return ConstructorDelegateCache[t].DynamicInvoke(args);
            
            var ctor = t.GetConstructors()
                .FirstOrDefault(c => c.GetParameters()
                    .Select(p => p.ParameterType)
                    .SequenceEqual(args.Select(ccc => ccc.GetType()))
                );

            if (ctor == null) throw new Exception("Unable to find constructor for these arguments");

            var argsExprs = Enumerable.Range(0, args.Length)
                .Select(i => Expression.Parameter(args[i]?.GetType() ?? throw new Exception(), $"arg{i}"))
                .ToList();

            // (arg0, arg1, arg2, etc) => new t(arg0, arg1, arg2, etc)
            var compiled = Expression.Lambda(Expression.New(ctor, argsExprs), argsExprs).Compile();
            
            ConstructorDelegateCache[t] = compiled;

            return compiled.DynamicInvoke(args);
        }
    }
}