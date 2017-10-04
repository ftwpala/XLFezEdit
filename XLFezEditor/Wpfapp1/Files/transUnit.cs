﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XLFezEditor.Files
{
    public class TransUnit : PropertyChangedBase
    {
        private readonly XElement element;
        private XNamespace xnamespace = "urn:oasis:names:tc:xliff:document:1.2"; // TODO Get namespace from document

        public TransUnit(XElement element)
        {
            this.element = element;
        }

        public string Source
        {
            get
            {
                return GetElementValue("source");
            }
        }
        public string Id
        {
            get
            {
                return element.Attribute("id").Value;
            }
        }


        public string Target
        {
            get
            {
                return GetElementValue("target");
            }
            set
            {
                element.Element(xnamespace + "target").Value = value;
                NotifyOfPropertyChange(()=>Target);

            }
        }

        public List<ContextGroup> Details
        {
            get
            {
                return element.Elements(xnamespace + "context-group").Select(cg => GetFileAndLineNumber(cg)).ToList();
            }
        }

        private ContextGroup GetFileAndLineNumber(XElement cg)
        {
            var source = GetElementValue("context-group", "context", "sourcefile");
            var lineNumber = GetElementValue("context-group", "context", "linenumber");
            var list = new ContextGroup(source, lineNumber);
            return list;
        }

        public string SourceFile
        {
            get
            {
               return GetElementValue("context-group", "context", "sourcefile");
            }
        }

        public string LineNumber
        {
            get
            {
                return GetElementValue("context-group", "context", "linenumber");
            }
        }

        public string Note
        {
            get
            {
                return GetElementValue("note", "description");
            }
        }

        private string GetElementValue(string nodeName)
        {

            XElement xElement = element.Element(xnamespace + nodeName);

            if (xElement == null)
            {
                return "";
            }
            else
            {
                var nodeValue = element.Element(xnamespace + nodeName).Nodes().Select(s => s.NodeType == XmlNodeType.Text ? SecurityElement.Escape(s.ToString()) : s.ToString());
                return string.Join("", nodeValue);
            }
        }

        private string GetElementValue(string nodeName, string attributeName)
        {
            return GetElementValue(element, nodeName, attributeName);
        }

        private string GetElementValue(XElement element, string nodeName, string attributeName)
        {
            if (element.HasElements)
            {
              List<XElement>  elementList = element.Elements().ToList();
                if (elementList == null)
                {
                    return "";
                }
                foreach (var listElement in elementList) // iterate through every element in the list
                {
                    if (listElement.HasAttributes)
                    {
                        var list = listElement.Attributes().ToList();
                        foreach (var item in list) // iterate through every attribute in the element
                        {
                            if (item.Value.Equals(attributeName))
                            {
                                return listElement.Value;
                            }
                        }
                    }

                }
                
            }
            else
            { 
                XElement xElement = element.Element(xnamespace + nodeName);
                if (xElement == null)
                {
                    return "";
                }
                if (xElement.HasAttributes)
                {
                    var list = xElement.Attributes().ToList();
                    foreach (var item in list)  // iterate through every attribute in the element
                    {
                        if (item.Value.Equals(attributeName))
                        {
                            return xElement.Value;
                        }
                    }
                }
            }

            Console.WriteLine("no element with attribute name " + attributeName + " found");
            return "";
        }
        private string GetElementValue(string nodeParentName, string nodeName, string attributeName)
        {

            XElement xElement = element.Element(xnamespace + nodeParentName);

            if (xElement == null)
            {
                return "";
            }
            else
            {
                return GetElementValue(xElement,nodeName, attributeName);
            }
        }

        private void SetTarget(string value)
        {
            XElement xElement = element.Element(xnamespace + "target");
            if (xElement == null)
            {
                element.Element(xnamespace + "source").AddAfterSelf(XElement.Parse("<target>" + value + "</target>"));
            }
            else
            {
                xElement.Value = value;
            }
        }
    }
}
