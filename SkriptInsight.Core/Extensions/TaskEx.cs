using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SkriptInsight.Core.Extensions
{
    public class TaskEx
    {
        [CanBeNull]
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
        {
            var task = function();
            if (task == null) return null;
            var resultTask = Task.Run(() => task);
            return resultTask.IsCanceled ? null : resultTask;
        }
    }
}