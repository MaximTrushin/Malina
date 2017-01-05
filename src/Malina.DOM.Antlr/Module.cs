using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

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
