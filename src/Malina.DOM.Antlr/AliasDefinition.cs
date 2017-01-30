#region license
// Copyright © 2016, 2017 Maxim Trushin (dba Syntactik, trushin@gmail.com, maxim.trushin@syntactik.com)
//
// This file is part of Malina.
// Malina is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Malina is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Malina.  If not, see <http://www.gnu.org/licenses/>.
#endregion
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Malina.DOM.Antlr
{
    public class AliasDefinition : DOM.AliasDefinition, IAntlrCharStreamConsumer, IValueNode
    {
        private NodeCollection<DOM.Parameter> _parameters;

        public ICharStream CharStream { get; set; }

        public Interval IdInterval { get; set; }

        public static AliasDefinition Undefined = new AliasDefinition(); //Used to if Alias has no corresponding Alias Definition.

        public bool HasDefaultBlockParameter { get; set; }
        public bool HasDefaultValueParameter { get; set; }
        public bool HasCircularReference { get; set; }

        public override string Name
        {
            get {
                return base.Name != null ? base.Name : CharStream.GetText(new Interval(IdInterval.a + 2, IdInterval.b));
            }
            set
            {
                base.Name = value;
            }
        }

        public NodeCollection<DOM.Parameter> Parameters
        {
            get { return _parameters ?? (_parameters = new NodeCollection<DOM.Parameter>(this)); }
            set
            {
                if (_parameters != null && value != _parameters)
                {
                    _parameters = value;
                }
            }
        }

        public Interval ValueInterval { get; set; }

        public int ValueIndent { get; set; }

        public override string Value
        {
            get
            {
                if (base.Value != null) return base.Value;
                return ValueType == ValueType.None ? null : Element.GetValueFromValueInterval(CharStream, ValueInterval, ValueIndent, ValueType);
            }
        }

        private List<object> _interpolationItems;
        public List<object> InterpolationItems => _interpolationItems ?? (_interpolationItems = new List<object>());
        
    }
}
