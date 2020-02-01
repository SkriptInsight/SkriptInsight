using System;

namespace SkriptInsight.JavaMetadataExtractorLib
{
    public class ObjectArgs<T> : EventArgs
    {
        public ObjectArgs(T o)
        {
            Object = o;
        }

        public T Object { get; set; }
    }
}