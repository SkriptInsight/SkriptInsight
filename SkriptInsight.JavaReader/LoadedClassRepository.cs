using System.Collections.Concurrent;
using System.Collections.Generic;
using Apache.NBCEL;

namespace SkriptInsight.JavaReader
{
    public class LoadedClassRepository
    {
        public static LoadedClassRepository Instance = new LoadedClassRepository();

        private ConcurrentDictionary<string, JavaClass> KnownClasses = new ConcurrentDictionary<string, JavaClass>();

        private LoadedClassRepository()
        {
        }

        public JavaClass this[string key]
        {
            get => KnownClasses.GetOrNull(key);
            set => KnownClasses[key] = value;
        }

        public void StoreClass(JavaClass clazz)
        {
            this[clazz.FullClassName] = clazz;
        }
    }
}