using System;
using System.Collections.Concurrent;
using SkriptInsight.Core.Files;

namespace SkriptInsight.Core.Extensions
{
    public class ConcurrentFileDictionary : ConcurrentDictionary<Uri, SkriptFile>
    {
        public new SkriptFile this[Uri key]
        {
            get => this.GetValue(key);
            set => base[key] = value;
        }
    }

}