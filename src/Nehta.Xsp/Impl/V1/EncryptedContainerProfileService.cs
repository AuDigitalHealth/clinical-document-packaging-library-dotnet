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
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Nehta.Xsp.Impl.Utils;
using Nehta.Common.Utils;


namespace Nehta.Xsp.Impl.V1
{
  /// <summary>
  /// Implementation of 'IEncryptedContainerProfileService' interface that supports
  /// the XML Secured Payload Profile.
  /// </summary>
  public class EncryptedContainerProfileService : IEncryptedContainerProfileService
  {
    /// <summary>
    /// 'encryptedPayload' tag name.
    /// </summary>    
    private const string EncryptedPayloadElement = "encryptedPayload";
    
    /// <summary>
    /// 'keys' tag name.
    /// </summary>
    private const string KeysElement = "keys";

    /// <summary>
    /// 'encryptedPayloadData' tag name.
    /// </summary>
    private const string EncryptedPayloadDataElement = "encryptedPayloadData";

    /// <summary>
    /// Xml namespace of the encrypted payload.
    /// </summary>
    private string encryptedPayloadXmlNs;
    
    /// <summary>
    /// Implementation of the encrypted profile service.
    /// </summary>
    private IXmlEncryptionProfileService encryptedProfileService;

    
    /// <summary>
    /// Constructor that sets the Xml namespace and the encrypted 
    /// profile service implementation.
    /// </summary>
    /// <param name="encryptedPayloadXmlNs">Xml namespace of the 
    /// encrypted payload.
    /// </param>
    /// <param name="encryptedProfileService">Implementation of the encrypted 
    /// profile service.</param>
    public EncryptedContainerProfileService(string encryptedPayloadXmlNs,
      IXmlEncryptionProfileService encryptedProfileService)
    {
      ArgumentUtils.CheckNotNull(encryptedPayloadXmlNs, 
        "encryptedPayloadXmlNs");
      ArgumentUtils.CheckNotNull(encryptedProfileService, 
        "encryptedProfileService");

      this.encryptedPayloadXmlNs = encryptedPayloadXmlNs;
      this.encryptedProfileService = encryptedProfileService;
    }

    /// <summary>
    /// Encrypts Xml data using one certificate and and creates an encrypted 
    /// payload container.
    /// </summary>
    /// <param name="payloadDoc">Payload to encrypt.</param>
    /// <param name="certificate">Certificate to encrypt with.</param>
    /// <returns>Encrypted container Xml document.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    public XmlDocument Create(XmlDocument payloadDoc, 
      X509Certificate2 certificate)
    {
      ArgumentUtils.CheckNotNull(payloadDoc, "payloadDoc");
      ArgumentUtils.CheckNotNull(certificate, "certificate");

      return Create(payloadDoc, new X509Certificate2Collection(certificate));
    }

    /// <summary>
    /// Encrypts Xml data using multiple certificates and creates an encrypted 
    /// payload container.
    /// </summary>
    /// <param name="payloadDoc">Payload to encrypt.</param>
    /// <param name="certificates">Certificates to encrypt with.</param>
    /// <returns>Encrypted container Xml document.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    public XmlDocument Create(XmlDocument payloadDoc, 
      X509Certificate2Collection certificates)
    {
        ArgumentUtils.CheckNotNull(payloadDoc, "payloadDoc");
        ArgumentUtils.CheckNotNull(certificates, "certificates");
        CertificateUtils.CheckNotNullOrEmpty(certificates, "certificates");

        return Create(payloadDoc, certificates, null);
    }

    /// <summary>
    /// Encrypts Xml data using multiple certificates and creates an encrypted 
    /// payload container.
    /// </summary>
    /// <param name="payloadDoc">Payload to encrypt.</param>
    /// <param name="certificates">Certificates to encrypt with.</param>
    /// <param name="cipherKey">The cipher to use when encrypting the payload.</param>
    /// <returns>Encrypted container Xml document.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    public XmlDocument Create(XmlDocument payloadDoc, X509Certificate2Collection certificates, byte[] cipherKey)
    {
        ArgumentUtils.CheckNotNull(payloadDoc, "payloadDoc");
        ArgumentUtils.CheckNotNull(certificates, "certificates");
        CertificateUtils.CheckNotNullOrEmpty(certificates, "certificates");

        // Create an empty Xml document to hold the elements      
        XmlDocument containerDoc = XmlUtils.CreateXmlDocument();

        // Create the 'ep:encryptedPayload' element
        XmlElement rootElem =
          containerDoc.CreateElement(EncryptedPayloadElement,
          this.encryptedPayloadXmlNs);
        containerDoc.AppendChild(rootElem);

        // Create the 'ep:keys' element
        XmlElement keysElem =
          containerDoc.CreateElement(KeysElement,
          this.encryptedPayloadXmlNs);
        // Add the 'ep:keys' element to the 'ep:encryptedPayload' element
        rootElem.AppendChild(keysElem);

        // Create the 'ep:encryptedPayloadData' element
        XmlElement dataElem =
          containerDoc.CreateElement(EncryptedPayloadDataElement,
          this.encryptedPayloadXmlNs);
        // Add the 'ep:encryptedPayloadData' element to the 'ep:encryptedPayload' 
        // element
        rootElem.AppendChild(dataElem);

        // Add the payload to the data element
        XmlNode payloadNode =
          containerDoc.ImportNode(payloadDoc.DocumentElement, true);
        dataElem.AppendChild(payloadNode);

        // Perform the encryption
        List<XmlElement> elementsToEncrypt = new List<XmlElement>();
        elementsToEncrypt.Add((XmlElement)payloadNode);
        this.encryptedProfileService.Encrypt(keysElem, elementsToEncrypt, certificates, cipherKey);

        // Return the created container
        return containerDoc;
    }

    /// <summary>
    /// Decrypts an encrypted container document returning the payload.
    /// </summary>
    /// <param name="encryptedPayloadDoc">Encrypted container to decrypt.</param>
    /// <param name="certificate">Certificate to decrypt with.</param>
    /// <returns>Decrypted payload.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    public XmlDocument GetData(XmlDocument encryptedPayloadDoc, 
      X509Certificate2 certificate)
    {
      ArgumentUtils.CheckNotNull(encryptedPayloadDoc, "encryptedPayloadDoc");
      ArgumentUtils.CheckNotNull(certificate, "certificate");

      return GetData(encryptedPayloadDoc.DocumentElement, certificate);
    }

    /// <summary>
    /// Decrypts an encrypted container document returning the payload.
    /// </summary>
    /// <param name="encryptedPayloadDoc">Encrypted container to decrypt.</param>
    /// <param name="cipherKey">The cipher key to use to decrypt the document.</param>
    /// <returns>Decrypted payload.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    public XmlDocument GetData(XmlDocument encryptedPayloadDoc, byte[] cipherKey)
    {
        ArgumentUtils.CheckNotNull(encryptedPayloadDoc, "encryptedPayloadDoc");
        ArgumentUtils.CheckNotNull(cipherKey, "cipherKey");

        XmlElement docElement = encryptedPayloadDoc.DocumentElement;

        // Check the root element belongs to the right namespace
        if (!XmlUtils.CheckElement(docElement, EncryptedPayloadElement,
          this.encryptedPayloadXmlNs))
        {
            throw new ArgumentException("Document is not an encrypted container");
        }

        // Clone the source element so it is not modified
        XmlElement clonePayloadElem = XmlUtils.Clone(docElement);

        // Create a namespace manager for XPath
        XmlNamespaceManager namespaceManager = CreateXmlNamespaceManager(
          clonePayloadElem.OwnerDocument);

        // Get the 'xenc:EncryptedData' element
        XmlElement encryptedDataElem = XPathUtils.GetElement(clonePayloadElem,
          "/ep:encryptedPayload/ep:encryptedPayloadData/xenc:EncryptedData",
          namespaceManager);
        if (encryptedDataElem == null)
        {
            throw new XspException("Encrypted data was not found within the container");
        }

        // Decrypt the element with the cipher key
        this.encryptedProfileService.Decrypt(encryptedDataElem, cipherKey);

        // Get the payload element
        XmlElement payloadElem = XPathUtils.GetElement(clonePayloadElem,
          "/ep:encryptedPayload/ep:encryptedPayloadData/*[1]", namespaceManager);

        // Return the payload element within a new Xml document
        return XmlUtils.CreateXmlDocument(payloadElem);
    }

    /// <summary>
    /// Decrypts an encrypted container Xml element, returning the payload.
    /// </summary>
    /// <param name="encryptedPayloadElem">Encrypted container to decrypt.</param>
    /// <param name="certificate">Certificate to decrypt with.</param>
    /// <returns>Decrypted payload.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    public XmlDocument GetData(XmlElement encryptedPayloadElem, 
      X509Certificate2 certificate)
    {
      ArgumentUtils.CheckNotNull(encryptedPayloadElem, "encryptedPayloadElem");
      ArgumentUtils.CheckNotNull(certificate, "certificate");
      
      // Check the root element belongs to the right namespace
      if (!XmlUtils.CheckElement(encryptedPayloadElem, EncryptedPayloadElement,
        this.encryptedPayloadXmlNs))
      {
        throw new ArgumentException("Document is not an encrypted container");
      }
      
      // Clone the source element so it is not modified
      XmlElement clonePayloadElem = XmlUtils.Clone(encryptedPayloadElem);
      
      // Create a namespace manager for XPath
      XmlNamespaceManager namespaceManager = CreateXmlNamespaceManager(
        clonePayloadElem.OwnerDocument);
      
      // Get the 'xenc:EncryptedData' element
      XmlElement encryptedDataElem = XPathUtils.GetElement(clonePayloadElem,
        "/ep:encryptedPayload/ep:encryptedPayloadData/xenc:EncryptedData", 
        namespaceManager);
      if (encryptedDataElem == null)
      {
        throw new XspException("Encrypted data was not found within the container");
      }
      
      // Get the 'xenc:EncryptedKey' element list
      IList<XmlElement> keyElems = XPathUtils.GetElements(clonePayloadElem,
        "/ep:encryptedPayload/ep:keys/xenc:EncryptedKey", namespaceManager);      
      if (keyElems.Count == 0)
      {
        throw new XspException("No encrypted keys found within the container");
      }
      
      // Decrypt the data
      this.encryptedProfileService.Decrypt(keyElems, encryptedDataElem, 
        certificate);
      
      // Get the payload element
      XmlElement payloadElem = XPathUtils.GetElement(clonePayloadElem,
        "/ep:encryptedPayload/ep:encryptedPayloadData/*[1]", namespaceManager);
      
      // Return the payload element within a new Xml document
      return XmlUtils.CreateXmlDocument(payloadElem);
    }
    
    /// <summary>
    /// Creates a namespace manager for XPathing.
    /// </summary>
    /// <param name="containerDoc">Context document.</param>
    /// <returns>Xml namespace manager.</returns>
    private XmlNamespaceManager CreateXmlNamespaceManager(XmlDocument containerDoc)
    {
      XmlNamespaceManager namespaceManager = new XmlNamespaceManager(
        containerDoc.NameTable);
      namespaceManager.AddNamespace("ep", this.encryptedPayloadXmlNs);
      namespaceManager.AddNamespace("xenc", EncryptedXml.XmlEncNamespaceUrl);
      return namespaceManager;    
    }
  }
}
