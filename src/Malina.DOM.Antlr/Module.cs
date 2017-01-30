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
namespace Malina.DOM.Antlr
{
    public class Module : DOM.Module
    {
        public enum TargetFormats
        {
            Undefined = 0,
            Xml,
            Json
        }

        private TargetFormats _targetFormat;
        public TargetFormats TargetFormat
        {
            get
            {
              if (_targetFormat != TargetFormats.Undefined) return _targetFormat;

              if (FileName != null && FileName.EndsWith(".mlj")) return _targetFormat = TargetFormats.Json;  

              return _targetFormat = TargetFormats.Xml;
            } 
            set { _targetFormat = value; }
        }

    }
}
