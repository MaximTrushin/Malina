using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
