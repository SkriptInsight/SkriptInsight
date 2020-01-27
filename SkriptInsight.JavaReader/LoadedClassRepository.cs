using System.Collections.Concurrent;
using System.Collections.Generic;

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
            get => KnownClasses/*.GetOrAdd(key, s => new JavaClass{FullClassName = s})*/[key];
            set => KnownClasses[key] = value;
        }

        public void StoreClass(JavaClass clazz)
        {
            this[clazz.FullClassName] = clazz;
        }
    }
}