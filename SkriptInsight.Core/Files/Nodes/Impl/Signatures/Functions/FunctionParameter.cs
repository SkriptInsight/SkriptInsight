using SkriptInsight.Core.Parser.Expressions;
using SkriptInsight.Core.SyntaxInfo;

namespace SkriptInsight.Core.Files.Nodes.Impl.Signatures.Functions
{
    public class FunctionParameter
    {
        public string Name
        {
            get => NameExpr.GenericValue;
            set => NameExpr.GenericValue = value;
        }

        public SkriptType Type
        {
            get => TypeExpr.GenericValue;
            set => TypeExpr.GenericValue = value;
        }

        public Expression<string> NameExpr { get; set; }

        public Expression<SkriptType> TypeExpr { get; set; }

        public IExpression DefaultValue { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Type.FinalTypeName}{RenderDefaultValue()}";
        }

        private string RenderDefaultValue()
        {
            return DefaultValue == null ? "" : $" = {DefaultValue.AsString()}";
        }
    }
}