using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Malina.DOM.Antlr
{
    public class AliasDefinition : DOM.AliasDefinition, IAntlrCharStreamConsumer, IValueNode
    {
        private NodeCollection<Parameter> _parameters;
        private ICharStream _charStream;
        private Interval _idInterval;
        private Interval _valueInterval;
        private int _valueIndent;

        public ICharStream CharStream
        {
            set
            {
                _charStream = value;
            }
        }

        public Interval IDInterval
        {
            set
            {
                _idInterval = value;
            }
        }

        public override string Name
        {
            get
            {
                if (base.Name != null) return base.Name;
                return _charStream.GetText(new Interval(_idInterval.a + 2, _idInterval.b));
            }

            set
            {
                base.Name = value;
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
                    _parameters = value;
                }
            }
        }

        public Interval ValueInterval
        {
            get
            {
                return _valueInterval;
            }

            set
            {
                _valueInterval = value;
            }
        }


        public int ValueIndent
        {
            get
            {
                return _valueIndent;
            }

            set
            {
                _valueIndent = value;
            }
        }

        public override string Value
        {
            get
            {
                if (base.Value != null) return base.Value;
                if (ValueType == ValueType.None) return null;
                return Element.GetValueFromValueInterval(_charStream, _valueInterval, _valueIndent, ValueType);
            }
        }

        private List<Alias> _interpolationAliases;
        public List<Alias> InterpolationAliases => _interpolationAliases ?? (_interpolationAliases = new List<Alias>());
    }
}
