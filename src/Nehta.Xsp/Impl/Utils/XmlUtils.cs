/*
 * Copyright 2010 NEHTA
 *
 * Licensed under the NEHTA Open Source (Apache) License; you may not use this
 * file except in compliance with the License. A copy of the License is in the
 * 'license.txt' file, which should be provided with this work.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */
using System.Collections.Generic;
using System.Xml;


namespace Nehta.Xsp.Impl.Utils
{
  /// <summary>
  /// Utility class that has Xml related methods.
  /// </summary>
  public static class XmlUtils
  {
    /// <summary>
    /// ID attribute name.
    /// </summary>
    public const string IdAttribute = "id";


    /// <summary>
    /// Adds an 'id' attribute to an element. The method does not check
    /// if there's already an existing 'id' attribute.
    /// </summary>
    /// <param name="element">Element to add the attribute. Cannot be null.
    /// </param>
    /// <param name="idVal">Value of the 'id' attribute. Cannot be null.</param>
    public static void AddIdAttribute(XmlElement element, string idVal)
    {
      XmlAttribute idAttr = element.OwnerDocument.CreateAttribute(IdAttribute);
      idAttr.Value = idVal;
      element.Attributes.Append(idAttr);
    }
    
    /// <summary>
    /// Creates an empty Xml document which preserves whitespace.
    /// </summary>
    /// <returns>Empty Xml document.</returns>
    public static XmlDocument CreateXmlDocument()
    {
      XmlDocument xmlDoc = new XmlDocument();
      xmlDoc.PreserveWhitespace = true;
      return xmlDoc;    
    }
    
    /// <summary>
    /// Creates an XML document which preserves whitespace and uses a
    /// specified element as the root node.
    /// </summary>
    /// <param name="rootElem">Element to be used as the root of the document.
    /// Cannot be null.</param>
    /// <returns>Xml document.</returns>
    public static XmlDocument CreateXmlDocument(XmlElement rootElem)
    {
      XmlDocument xmlDoc = CreateXmlDocument();
      XmlNode importedNode = xmlDoc.ImportNode(rootElem, true);
      xmlDoc.AppendChild(importedNode);

      return xmlDoc;
    }
    
    /// <summary>
    /// Clones an element. The cloned element is within a new document.
    /// </summary>
    /// <param name="srcElement">Source element to clone. Cannot be
    /// null.</param>
    /// <returns>Cloned element in a new Xml document.</returns>
    public static XmlElement Clone(XmlElement srcElement)
    {
      return CreateXmlDocument(srcElement).DocumentElement;
    }

    /// <summary>
    /// Clones a document.
    /// </summary>
    /// <param name="srcDoc">Source document to clone. Cannot be null.</param>
    /// <returns>Cloned document.</returns>
    public static XmlDocument Clone(XmlDocument srcDoc)
    {
      return CreateXmlDocument(srcDoc.DocumentElement);
    }

    /// <summary>
    /// Loads an XML document preserving any whitespace.
    /// </summary>
    /// <param name="path">Path to the Xml document. Cannot
    /// be null.</param>
    /// <returns>Loaded Xml document.</returns>
    public static XmlDocument LoadXmlDocument(string path)
    {
      XmlDocument xmlDoc = CreateXmlDocument();
      xmlDoc.Load(path);      
      return xmlDoc;
    }

    /// <summary>
    /// Checks if an element matches the name and namespace.
    /// </summary>
    /// <param name="elem">Element to check. Cannot be null.</param>
    /// <param name="elemName">Expected element name. Cannot be null.
    /// </param>
    /// <param name="elemNamespace">Expected element namespace. Cannot 
    /// be null.</param>
    /// <returns>True if the name and namespace match otherwise false.
    /// </returns>
    public static bool CheckElement(XmlElement elem, string elemName,
      string elemNamespace)
    {
      return (elem.LocalName == elemName &&
        elem.NamespaceURI == elemNamespace);
    }

    /// <summary>
    /// Converts a node list to an element list.
    /// </summary>
    /// <param name="nodeList">Node list to convert. Cannot be null or empty.
    /// </param>
    /// <returns>Element list.</returns>
    public static IList<XmlElement> ConvertToElementList(XmlNodeList nodeList)
    {
      IList<XmlElement> elemsList = new List<XmlElement>();
      foreach (XmlElement node in nodeList)
      {
        elemsList.Add(node);
      }

      return elemsList;
    }
    
    /// <summary>
    /// Returns true if the element to check is a descendant of the 
    /// source element.
    /// </summary>
    /// <param name="sourceElem">Source element to start from. Cannot be null.
    /// </param>
    /// <param name="checkElem">Element to check. Cannot be null.</param>
    /// <returns>True if the element is a descendant otherwise false.</returns>
    public static bool IsDescendant(XmlElement sourceElem, XmlElement checkElem)
    {
      return sourceElem.CreateNavigator().IsDescendant(checkElem.CreateNavigator());
    }
  }
}
