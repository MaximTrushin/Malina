﻿#region license
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
using System.Xml;
using System.Xml.Schema;
using Malina.DOM;

namespace Malina.Compiler.Generator
{
    public class SourceMappedXmlValidator
    {
        private int _validationIndex;
        private Stack<int> _indicesStack;
        private List<LexicalInfo> LocationMap { get; }
        private XmlSchemaSet XmlSchemaSet { get; }

        public delegate void ValidationEventHandler(CompilerError error);

        public event ValidationEventHandler ValidationErrorEvent;

        public SourceMappedXmlValidator(List<LexicalInfo> locationMap, XmlSchemaSet xmlSchemaSet)
        {
            LocationMap = locationMap;
            XmlSchemaSet = xmlSchemaSet;
        }

        public void ValidateGeneratedFile(string fileName)
        {
            try
            {
                var settings = new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Document,
                    ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes |
                                      XmlSchemaValidationFlags.ReportValidationWarnings,
                    Schemas = XmlSchemaSet
                };

                if (XmlSchemaSet.Count > 0) settings.ValidationType = ValidationType.Schema;

                _validationIndex = 0;
                _indicesStack = new Stack<int>();
                _indicesStack.Push(0);
                settings.ValidationEventHandler += InternalValidationEventHandler;

                using (var textReader = new XmlTextReader(fileName))
                {
                    using (var reader = XmlReader.Create(textReader, settings))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (!reader.IsEmptyElement)
                                    _indicesStack.Push(_validationIndex); //This stack is used to store index of the current parent element.

                                _validationIndex++;
                                _validationIndex += GetAttributesCount(reader);
                            }

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                _indicesStack.Pop();
                            }
                        }
                    }
                }
            }
            catch (XmlSchemaValidationException ex)
            {
                ValidationErrorEvent?.Invoke(CompilerErrorFactory.XmlSchemaValidationError(ex));
            }
        }

        private void InternalValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity != XmlSeverityType.Error) return;

            var index = ((XmlReader)sender).NodeType == XmlNodeType.EndElement ? _indicesStack.Peek() : _validationIndex;

            var location = LocationMap[index];
            ValidationErrorEvent?.Invoke(CompilerErrorFactory.XmlSchemaValidationError(e.Exception, location));
        }

        private static int GetAttributesCount(XmlReader reader)
        {
            if (!reader.MoveToFirstAttribute())
                return 0;
            var count = 0;
            do
            {
                if (reader.NamespaceURI != "http://www.w3.org/2000/xmlns/")
                    count++;
            } while (reader.MoveToNextAttribute());
            return count;
        }
    }
}
