namespace Malina.DOM
{
    public interface IDomVisitor
    {
        // Methods
        void OnAlias(Alias node);
        void OnAliasDefinition(AliasDefinition node);
        void OnArgument(Argument node);
        void OnAttribute(Attribute node);
        void OnDocument(Document node);
        void OnElement(Element node);
        void OnModule(Module node);
        void OnNamespace(Namespace node);
        void OnNamespaceScope(NamespaceScope node);
        void OnParameter(Parameter node);
    }



}