using System;
using System.Collections.Generic;

namespace Malina.DOM
{
    [Serializable]
    public class AliasDefinition : ModuleMember
    {
        // Fields
        private NodeCollection<Attribute> _attributes;
        private NodeCollection<Entity> _entities;
        private NodeCollection<Namespace> _namespaces;
        private NodeCollection<Parameter> _parameters;
        public string Value;
        public ValueType ValueType;

        // Methods
        public AliasDefinition()
        {
        }

        public AliasDefinition(string name)
        {
            Name = name;
        }

        public AliasDefinition(List<string> ns, string name)
        {
            string str = string.Empty;
            if (ns.Count > 0)
            {
                str = string.Join(".", ns) + ".";
            }
            Name = str + name;
        }

        public AliasDefinition(string name, NodeCollection<Namespace> namespaces, NodeCollection<Attribute> attributes, NodeCollection<Entity> entities, NodeCollection<Parameter> parameters, string value)
        {
            Name = name;
            Namespaces = namespaces;
            Attributes = attributes;
            Entities = entities;
            Parameters = parameters;
            Value = value;
        }

        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnAliasDefinition(this);
        }

        public override void AppendChild(Node child)
        {
            child.OwnerModule = base.OwnerModule;
            if (child is Attribute)
            {
                Attributes.Add((Attribute)child);
            }
            else if (child is Parameter)
            {
                Entities.Add((Entity)child);
                Parameters.Add((Parameter)child);
            }
            else if (child is Namespace)
            {
                Namespaces.Add((Namespace)child);
            }
            else if (child is Entity)
            {
                Entities.Add((Entity)child);
            }
            else
            {
                base.AppendChild(child);
            }
        }

        public override void Assign(Node node, bool shallow)
        {
            base.Assign(node, shallow);
            AliasDefinition definition = node as AliasDefinition;
            Value = definition.Value;
            Namespaces.AssignNodes(definition.Namespaces);
            if (!shallow)
            {
                Entities.AssignNodes(definition.Entities);
                Attributes.AssignNodes(definition.Attributes);
                Parameters.AssignNodes(definition.Parameters);
            }
        }

        public override Node Clone()
        {
            AliasDefinition definition = new AliasDefinition();
            definition.Assign(this, false);
            return definition;
        }

        // Properties
        public NodeCollection<Attribute> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new NodeCollection<Attribute>(this);
                }
                return _attributes;
            }
            set
            {
                if (value != _attributes)
                {
                    if (value != null)
                    {
                        value.InitializeParent(this);
                    }
                    _attributes = value;
                }
            }
        }

        public NodeCollection<Entity> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = new NodeCollection<Entity>(this);
                }
                return _entities;
            }
            set
            {
                if (value != _entities)
                {
                    if (value != null)
                    {
                        value.InitializeParent(this);
                    }
                    _entities = value;
                }
            }
        }

        public Module Module
        {
            get
            {
                return (Parent as Module);
            }
        }

        public NodeCollection<Namespace> Namespaces
        {
            get
            {
                if (_namespaces == null)
                {
                    _namespaces = new NodeCollection<Namespace>(this);
                }
                return _namespaces;
            }
            set
            {
                if (value != _namespaces)
                {
                    if (value != null)
                    {
                        value.InitializeParent(this);
                    }
                    _namespaces = value;
                }
            }
        }

        public NodeCollection<Parameter> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new NodeCollection<Parameter>(this);
                }
                return _parameters;
            }
            set
            {
                if (value != _parameters)
                {
                    if (value != null)
                    {
                        value.InitializeParent(this);
                    }
                    _parameters = value;
                }
            }
        }
    }


}