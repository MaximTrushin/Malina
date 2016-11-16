using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malina.DOM
{
    [Serializable]
    public class Document : ModuleMember
    {
        // Fields
        private NodeCollection<Entity> _entities;
        
        public Entity DocumentElement;

        // Methods
        public Document()
        {
        }


        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnDocument(this);
        }

        public override void AppendChild(Node child)
        {
            if (child is Entity)
            {
                DocumentElement = (Entity)child;
                child.InitializeParent(this);
                Entities.Add((Entity)child);
            }
            else if (child is Namespace)
            {
                Namespaces.Add((Namespace)child);
            }
            else
            {
                base.AppendChild(child);
            }
        }

        public override void Assign(Node node)
        {
            base.Assign(node);
            Document document = node as Document;
            Namespaces.AssignNodes(document.Namespaces);
            DocumentElement = (Entity)document.DocumentElement.Clone();

        }

        public override Node Clone()
        {
            Document document = new Document();
            document.Assign(this);
            return document;
        }

        // Properties
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

    }


}