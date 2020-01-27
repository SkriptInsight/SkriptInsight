using System.Collections.Generic;
using Apache.NBCEL.ClassFile;
using Apache.NBCEL.Generic;

namespace SkriptInsight.JavaReader
{
    public class JavaMethodParameter
    {
        public JavaMethodParameter()
        {
            
        }
        
        protected JavaMethodParameter(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public override string ToString()
        {
            return $"{Type} {Name}";
        }

        public virtual string Name { get; set; }

        public virtual Type Type { get; set; }
        
        public static JavaMethodParameter[] FromMethod(Method method)
        {
            var parameters = new List<JavaMethodParameter>();
            var vars = method.LocalVariableTable;

            var variableNameIndex = method.IsStatic() ? 0 : 1;
            var normalIndex = 0;
            foreach (var argumentType in method.ArgumentTypes)
            {
                var argName = $"arg{++normalIndex}";
                var localVariable = vars?.GetLocalVariable(variableNameIndex, 0);
                if (localVariable != null)
                {
                    argName = localVariable.GetName();
                }

                parameters.Add(new JavaMethodParameter(argName, argumentType));
                
                variableNameIndex += argumentType.GetSize();
            }

            return parameters.ToArray();
        }
    }
}