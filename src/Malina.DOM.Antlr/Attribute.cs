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
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;

namespace Malina.DOM.Antlr
{
    public class Attribute : DOM.Attribute, IAntlrCharStreamConsumer, IValueNode
    {
        private ICharStream _charStream;
        private int _nsSeparator = -2;

        public ICharStream CharStream
        {
            set
            {
                _charStream = value;
            }
        }

        public Interval IdInterval { get; set; }

        public Interval ValueInterval { get; set; } = Interval.Invalid;

        public override string Name
        {
            get
            {
                return base.Name ??
                       _charStream.GetText(NsSeparator > 0 ? new Interval(NsSeparator, IdInterval.b) : new Interval(IdInterval.a + 1, IdInterval.b)).Replace("..", ".");
            }

            set
            {
                base.Name = value;
            }
        }

        public override string Value => base.Value ??
                                        Element.GetValueFromValueInterval(_charStream, ValueInterval, ValueIndent, ValueType);

        public override void AppendChild(Node child)
        {
            ObjectValue = child;
        }
        public int ValueIndent { get; set; }

        public int NsSeparator
        {
            get
            {
                if (_nsSeparator == -2)
                {
                    //Calculate NsSeparator
                    _nsSeparator = Element.CalcNsSeparator(_charStream, IdInterval);
                }
                return _nsSeparator;
            }

            set
            {
                _nsSeparator = value;
            }
        }

        private List<object> _interpolationItems;
        public List<object> InterpolationItems => _interpolationItems ?? (_interpolationItems = new List<object>());

        public override string NsPrefix
        {
            get
            {
                if (base.NsPrefix != null) return base.NsPrefix;
                return NsSeparator > 0 ? _charStream.GetText(new Interval(IdInterval.a + 1, NsSeparator - 2)) : null;
            }
        }
    }
}
