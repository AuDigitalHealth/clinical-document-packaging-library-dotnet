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
  /// Implementation of 'ISignedContainerProfileService' interface that supports
  /// XML Secured Payload Profile.
  /// </summary>
  public class SignedContainerProfileService : ISignedContainerProfileService
  {
    /// <summary>
    /// 'signedPayload' tag name.
    /// </summary>
    private const string SignedPayloadElement = "signedPayload";
  
    /// <summary>
    /// 'signatures' tag name.
    /// </summary>
    private const string SignaturesElement = "signatures";
    
    /// <summary>
    /// 'signedPayloadData' tag name.
    /// </summary>
    private const string SignedPayloadDataElement = "signedPayloadData";

    /// <summary>
    /// Xml namespace of the signed payload.
    /// </summary>
    private string signedPayloadXmlNs;

    /// <summary>
    /// Implementation of the signed profile service.
    /// </summary>
    private IXmlSignatureProfileService signedProfileService;


    /// <summary>
    /// Constructor that sets the Xml namespace and the signed 
    /// profile service implementation.
    /// </summary>
    /// <param name="signedPayloadXmlNs">Xml namespace of the 
    /// signed payload.
    /// </param>
    /// <param name="signedProfileService">Implementation of the signed 
    /// profile service.</param>
    public SignedContainerProfileService(string signedPayloadXmlNs,
      IXmlSignatureProfileService signedProfileService)
    {
      ArgumentUtils.CheckNotNull(signedPayloadXmlNs, "signedPayloadXmlNs");
      ArgumentUtils.CheckNotNull(signedProfileService, "signedProfileService");

      this.signedPayloadXmlNs = signedPayloadXmlNs;
      this.signedProfileService = signedProfileService;
    }

    /// <summary>
    /// Signs Xml data with a certificate and creates a signed payload container.
    /// </summary>
    /// <param name="payloadDoc">Document to sign.</param>
    /// <param name="certificate">Certificate used to sign with.</param>
    /// <returns>Signed container.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    public XmlDocument Create(XmlDocument payloadDoc, X509Certificate2 certificate)
    {
      ArgumentUtils.CheckNotNull(payloadDoc, "payloadDoc");
      ArgumentUtils.CheckNotNull(certificate, "certificate");

      return Create(payloadDoc, new X509Certificate2Collection(certificate));
    }

    /// <summary>
    /// Signs Xml data with multiple certificates and creates a signed payload container.
    /// </summary>
    /// <param name="payloadDoc">Document to sign.</param>
    /// <param name="certificates">List of certificates used to sign with.</param>
    /// <returns>Signed container.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    public XmlDocument Create(XmlDocument payloadDoc, 
      X509Certificate2Collection certificates)
    {
      ArgumentUtils.CheckNotNull(payloadDoc, "payloadDoc");
      CertificateUtils.CheckNotNullOrEmpty(certificates, "certificates");
      
      // Create the container
      XmlDocument containerDoc = XmlUtils.CreateXmlDocument();

      // Create the 'sp:signedPayload' root element of the container
      XmlElement rootElem =
        containerDoc.CreateElement(SignedPayloadElement,
        this.signedPayloadXmlNs);
      containerDoc.AppendChild(rootElem);

      // Create the 'sp:signatures' element
      XmlElement signaturesElem =
        containerDoc.CreateElement(SignaturesElement,
        this.signedPayloadXmlNs);
      // Add the 'sp:signatures' to the 'sp:signedPayload' element
      rootElem.AppendChild(signaturesElem);

      // Create the 'sp:signedPayloadData' element
      XmlElement dataElem =
        containerDoc.CreateElement(SignedPayloadDataElement,
        this.signedPayloadXmlNs);
      // Add the data element 'sp:signedPayloadData' to 'sp:signedPayload' element
      rootElem.AppendChild(dataElem);
      
      // Add the payload data
      XmlNode payloadNode = containerDoc.ImportNode(payloadDoc.DocumentElement, true);
      dataElem.AppendChild(payloadNode);

      // Add an 'id' attribute to the data element for use as a signing reference
      // Make sure reference starts with an alpha char - 'Malformed message' error if starts with number
      string referenceId = "Id_" + Guid.NewGuid().ToString();
      XmlUtils.AddIdAttribute(dataElem, referenceId);

      this.signedProfileService.Sign(signaturesElem, dataElem, certificates);

      return containerDoc;
    }

    /// <summary>
    /// Extracts the payload from the signed payload container.
    /// </summary>
    /// <param name="containerDoc">Signed payload container.</param>
    /// <returns>Payload.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    public XmlDocument GetData(XmlDocument containerDoc)
    {
      ArgumentUtils.CheckNotNull(containerDoc, "containerDoc");

      // Check the document is a signed container
      if (!XmlUtils.CheckElement(containerDoc.DocumentElement, SignedPayloadElement,
        this.signedPayloadXmlNs))
      {
        throw new ArgumentException("Document is not a signed container");
      }
      
      // Create an Xml namespace manager for XPathing
      XmlNamespaceManager namespaceManager = CreateXmlNamespaceManager(
        containerDoc);
      
      // Get the data from the container
      XmlElement payloadDataElem = XPathUtils.GetElement(containerDoc,
        "/sp:signedPayload/sp:signedPayloadData/*[1]", namespaceManager);
      
      return XmlUtils.CreateXmlDocument(payloadDataElem);      
    }

    /// <summary>
    /// Checks the signatures in a signed payload container and also verifies the 
    /// certificates with the certificate verifier callback.
    /// </summary>
    /// <param name="containerDoc">Signed payload container.</param>
    /// <param name="certificateVerifier">Certificate verifier.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    public void Check(XmlDocument containerDoc, 
      ICertificateVerifier certificateVerifier)
    {
      ArgumentUtils.CheckNotNull(containerDoc, "containerDoc");
      ArgumentUtils.CheckNotNull(certificateVerifier, "certificateVerifier");
      
      // Check the container is valid
      CheckSignedContainer(containerDoc);
            
      // Get all the signatures in the container
      IList<XmlElement> signatureElems = XPathUtils.GetElements(containerDoc,
        "/sp:signedPayload/sp:signatures/ds:Signature", 
        CreateXmlNamespaceManager(containerDoc));
      
      // Check all the signatures verify
      this.signedProfileService.Check(signatureElems, certificateVerifier);
    }

    /// <summary>
    /// Returns a list of certificates from the signed container.
    /// </summary>
    /// <param name="containerDoc">Signed container.</param>
    /// <returns>List of certificates.</returns>     
    public IList<X509Certificate2> GetSigningCertificates(XmlDocument containerDoc)
    {
      ArgumentUtils.CheckNotNull(containerDoc, "containerDoc");

      // Check the document is a signed container
      if (!XmlUtils.CheckElement(containerDoc.DocumentElement, SignedPayloadElement,
        this.signedPayloadXmlNs))
      {
        throw new ArgumentException("Document is not a signed container");
      }

      // Create the namespace manager for executing XPath statements
      XmlNamespaceManager namespaceManager = CreateXmlNamespaceManager(
        containerDoc);
      
      // Get the list of signature elements
      IList<XmlElement> signatureElems = XPathUtils.GetElements(containerDoc, 
        "/sp:signedPayload/sp:signatures/ds:Signature", namespaceManager);
    
      if (signatureElems.Count == 0)
      {
        throw new XspException("No 'ds:Signature' elements were found " +
          "within the 'sp:signatures' element");
      }
    
      IList<X509Certificate2> certs = new List<X509Certificate2>();
      foreach (XmlElement signatureElem in signatureElems)
      {
        // Add each certificate 
        X509Certificate2 cert = 
          this.signedProfileService.GetSigningCertificate(signatureElem);
          
        certs.Add(cert);
      }
          
      return certs;
    }

    /// <summary>
    /// Gets a list of digest values.
    /// </summary>
    /// <param name="containerDoc">Signed container.</param>
    /// <returns>List of digest values.</returns>
    public IList<byte[]> GetDigestValues(XmlDocument containerDoc) 
    {
      ArgumentUtils.CheckNotNull(containerDoc, "containerDoc");

      // Check the document is a signed container
      if (!XmlUtils.CheckElement(containerDoc.DocumentElement, SignedPayloadElement,
        this.signedPayloadXmlNs))
      {
        throw new ArgumentException("Document is not a signed container");
      }

      // Create the namespace manager for executing XPath statements
      XmlNamespaceManager namespaceManager = CreateXmlNamespaceManager(
        containerDoc);

      // Get the list of signature elements
      IList<XmlElement> signatureElems = XPathUtils.GetElements(containerDoc,
        "/sp:signedPayload/sp:signatures/ds:Signature", namespaceManager);
      
      if (signatureElems.Count == 0)
      {
        throw new XspException("No 'ds:Signature' elements were found within the " +
          "'sp:signatures' element");
      }
      
      // Iterate through each signature to get the digest
      IList<byte[]> digestList = new List<byte[]>();
      foreach (XmlElement signatureElem in signatureElems)
      {
        // Extract the digest from the signature
        IDictionary<string, byte[]> digestValues = 
          this.signedProfileService.GetDigestValues(signatureElem);  

        if (digestValues.Keys.Count == 0)
        {
          throw new XspException("Signature contains no refences");
        }
        
        if (digestValues.Keys.Count > 1)
        {
          throw new XspException("Signature contains more than one reference");
        }
        
        // Get the first value from the values list as there is only one
        IEnumerator<byte[]> digestEnumerator = 
          digestValues.Values.GetEnumerator();
        digestEnumerator.MoveNext();
        digestList.Add(digestEnumerator.Current);
      }

      return digestList;
    }
    
    /// <summary>
    /// Creates an Xml namespace manager for XPathing.
    /// </summary>
    /// <param name="containerDoc">Context document.</param>
    /// <returns>Xml namespace manager object.</returns>
    private XmlNamespaceManager CreateXmlNamespaceManager(
      XmlDocument containerDoc)
    {
      // Create the namespace manager for executing XPath statements
      XmlNamespaceManager namespaceManager = new XmlNamespaceManager(
        containerDoc.NameTable);
      namespaceManager.AddNamespace("sp", this.signedPayloadXmlNs);
      namespaceManager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl); 
      
      return namespaceManager;      
    }
    
    /// <summary>
    /// Checks the container is valid.
    /// </summary>
    /// <param name="containerDoc">Signed container.</param>
    /// <exception cref="ArgumentException">Thrown when the container is not valid.
    /// </exception>
    private void CheckSignedContainer(XmlDocument containerDoc)
    {
      // Check the document is a signed container
      if (!XmlUtils.CheckElement(containerDoc.DocumentElement, SignedPayloadElement,
        this.signedPayloadXmlNs))
      {
        throw new ArgumentException("Document is not a signed container");
      }
      
      XmlNamespaceManager namespaceManager = 
        CreateXmlNamespaceManager(containerDoc);

      // Check the container has signatures
      IList<XmlElement> signatureElems = XPathUtils.GetElements(containerDoc,
        "/sp:signedPayload/sp:signatures/ds:Signature", namespaceManager);
      if (signatureElems.Count == 0)
      {
        throw new XspException("No signatures were found in the signed container");
      }

      // Check the container contains data
      if (XPathUtils.GetElement(containerDoc,
        "/sp:signedPayload/sp:signedPayloadData", namespaceManager) == null)
      {
        throw new XspException("No data section within the container");
      }

    }

  }
}
