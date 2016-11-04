namespace Malina.DOM
{
    public interface IDomVisitor
    {
        // Methods
        void OnAlias(Alias node);
        void OnAliasDefinition(AliasDefinition node);
        void OnArgument(Argument node);
        void OnAttribute(Attribute node);
        void OnCompileUnit(CompileUnit node);
        void OnDocument(Document node);
        void OnElement(Element node);
        void OnModule(Module node);
        void OnNamespace(Namespace node);
        void OnScope(Scope node);
        void OnParameter(Parameter node);
    }



}