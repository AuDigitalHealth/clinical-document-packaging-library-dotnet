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
  /// Utility class with XPath related methods.
  /// </summary>
  public static class XPathUtils
  {
    /// <summary>
    /// Checks if an element exists at an XPath.
    /// </summary>
    /// <param name="xmlDoc">Context document. Cannot be null.</param>
    /// <param name="xpath">XPath to check. Cannot be null.</param>
    /// <param name="nsManager">Namespace manager. Cannot be null.</param>
    /// <returns>True if the element exists otherwise false.</returns>
    public static bool DoesElementExist(XmlDocument xmlDoc, 
      string xpath, XmlNamespaceManager nsManager)
    {
      return (xmlDoc.SelectSingleNode(xpath, nsManager) != null);
    }
    
    /// <summary>
    /// Returns the number of elements at an XPath.
    /// </summary>
    /// <param name="xmlDoc">Context document. Cannot be null.</param>
    /// <param name="xpath">XPath to use. Cannot be null.</param>
    /// <param name="nsManager">Namespace manager. Cannot be null.</param>
    /// <returns>Number of elements.</returns>
    public static int GetCount(XmlDocument xmlDoc, string xpath,
      XmlNamespaceManager nsManager)
    {
      return (xmlDoc.SelectNodes(xpath, nsManager).Count);    
    }
    
    /// <summary>
    /// Gets a list of elements at an XPath.
    /// </summary>
    /// <param name="elem">Root context element. Cannot be null.</param>
    /// <param name="xpath">XPath to use. Cannot be null.</param>
    /// <param name="nsManager">Namespace manager. Cannot be null.</param>
    /// <returns>List of elements or an empty list.</returns>
    public static IList<XmlElement> GetElements(XmlElement elem,
      string xpath, XmlNamespaceManager nsManager)
    {
      XmlNodeList nodeList = elem.SelectNodes(xpath, nsManager);
      
      return XmlUtils.ConvertToElementList(nodeList);
    }

    /// <summary>
    /// Gets a list of elements at an XPath.
    /// </summary>
    /// <param name="xmlDoc">Root context document. Cannot be null.</param>
    /// <param name="xpath">XPath to use. Cannot be null.</param>
    /// <param name="nsManager">Namespace manager.</param>
    /// <returns>List of elements or an empty list.</returns>  
    public static IList<XmlElement> GetElements(XmlDocument xmlDoc,
      string xpath, XmlNamespaceManager nsManager)
    {      
      return GetElements(xmlDoc.DocumentElement, xpath, nsManager);
    }
    
    /// <summary>
    /// Gets an element at an XPath.
    /// </summary>
    /// <param name="elem">Root context element. Cannot be null.</param>
    /// <param name="xpath">XPath to use. Cannot be null.</param>
    /// <param name="nsManager">Namespace manager. Cannot be null.</param>
    /// <returns>Element at the path otherwise null.</returns>
    public static XmlElement GetElement(XmlElement elem, string xpath,
      XmlNamespaceManager nsManager)
    {
      return (XmlElement)elem.SelectSingleNode(xpath, nsManager);
    }

    /// <summary>
    /// Gets an element at an XPath.
    /// </summary>
    /// <param name="xmlDoc">Root context document. Cannot be null.</param>
    /// <param name="xpath">XPath to use. Cannot be null.</param>
    /// <param name="nsManager">Namespace manager. Cannot be null.</param>
    /// <returns>Element at the path otherwise null.</returns>  
    public static XmlElement GetElement(XmlDocument xmlDoc, string xpath,
      XmlNamespaceManager nsManager)
    {
      return GetElement(xmlDoc.DocumentElement, xpath, nsManager);
    }
  
  }
}
