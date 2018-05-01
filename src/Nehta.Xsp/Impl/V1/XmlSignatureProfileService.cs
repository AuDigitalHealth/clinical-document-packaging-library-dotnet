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
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Nehta.Xsp.Impl.Utils;
using Nehta.Common.Utils;


namespace Nehta.Xsp.Impl.V1
{
  /// <summary>
  /// Implementation of 'ISignedProfileService' interface that supports
  /// XML Secured Payload Profile.
  /// </summary>
  public class XmlSignatureProfileService : IXmlSignatureProfileService
  {
    /// <summary>
    /// 'id' attribute name.
    /// </summary>
    private static string XmlIdLocalName = "id";

    /// <summary>
    /// Signature element name.
    /// </summary>
    private static string SignatureElement = "Signature";


    /// <summary>
    /// Signs an element.
    /// </summary>
    /// <param name="elementToAddToSigTo">Element to add the signature to.</param>
    /// <param name="elementToSign">Element to sign.</param>
    /// <param name="certificate"></param>
    public void Sign(XmlElement elementToAddToSigTo, XmlElement elementToSign, 
      X509Certificate2 certificate)
    {
      ArgumentUtils.CheckNotNull(elementToAddToSigTo, "elementToAddToSigTo");
      ArgumentUtils.CheckNotNull(elementToSign, "elementToSign");
      ArgumentUtils.CheckNotNull(certificate, "certificate");

      X509Certificate2Collection certificates = new X509Certificate2Collection(
        certificate);
      IList<XmlElement> elementsToSign = new List<XmlElement>();
      elementsToSign.Add(elementToSign);
      Sign(elementToAddToSigTo, elementsToSign, certificates);
    }

    /// <summary>
    /// Signs a list of elements.
    /// </summary>
    /// <param name="elementToAddToSigTo"></param>
    /// <param name="elementsToSign"></param>
    /// <param name="certificate"></param>
    public void Sign(XmlElement elementToAddToSigTo, 
      IList<XmlElement> elementsToSign, X509Certificate2 certificate)
    {
      ArgumentUtils.CheckNotNull(elementToAddToSigTo, "elementToAddToSigTo");
      ArgumentUtils.CheckNotNullNorEmpty(elementsToSign, "elementsToSign");
      ArgumentUtils.CheckNotNull(certificate, "certificate");

      X509Certificate2Collection certificates = new X509Certificate2Collection(
        certificate);
      Sign(elementToAddToSigTo, elementsToSign, certificates);
    }

    /// <summary>
    /// Signs an element.
    /// </summary>
    /// <param name="elementToAddToSigTo"></param>
    /// <param name="elementToSign"></param>
    /// <param name="certificates"></param>
    public void Sign(XmlElement elementToAddToSigTo, XmlElement elementToSign, 
      X509Certificate2Collection certificates)
    {
      ArgumentUtils.CheckNotNull(elementToAddToSigTo, "elementToAddToSigTo");
      ArgumentUtils.CheckNotNull(elementToSign, "elementToSign");
      CertificateUtils.CheckNotNullOrEmpty(certificates, "certificates");

      IList<XmlElement> elementsToSign = new List<XmlElement>();
      elementsToSign.Add(elementToSign);
      Sign(elementToAddToSigTo, elementsToSign, certificates);
    }

    /// <summary>
    /// Signs a list of elements.
    /// </summary>
    /// <param name="elementToAddToSigTo"></param>
    /// <param name="elementsToSign"></param>
    /// <param name="certificates"></param>
    public void Sign(XmlElement elementToAddToSigTo, 
      IList<XmlElement> elementsToSign, X509Certificate2Collection certificates)
    {
      ArgumentUtils.CheckNotNull(elementToAddToSigTo, "elementToAddToSigTo");
      ArgumentUtils.CheckNotNullNorEmpty(elementsToSign, "elementsToSign");
      CertificateUtils.CheckNotNullOrEmpty(certificates, "certificates");
      
      // Get the owner document
      XmlDocument containerDoc = elementToAddToSigTo.OwnerDocument;
      
      // Check each certificate has a private key
      foreach (X509Certificate2 cert in certificates)
      {
        if (cert.PrivateKey == null)
        {
          throw new XspException("Certificate with subject '" + 
            cert.Subject + "' does not contain a private key");
        }
      }
      
      // Check the 'elementsToSign' elements
      foreach (XmlElement elementToSign in elementsToSign)
      {
        // Check an element to sign is not the same as the element where the
        // signature will be added
        if (elementToSign == elementToAddToSigTo)
        {
          throw new XspException("Cannot add the signature to an " + 
            "element being signed");
        }
        
        // Check if all elements have the same owner document
        if (elementToSign.OwnerDocument != containerDoc)
        {
          throw new XspException("Element to sign must belong to the same " +
            "document to where the signature is being added");
        }
        
        // Check the element to add the signature to is not a descendant
        // of an element being signed
        if (XmlUtils.IsDescendant(elementToSign, elementToAddToSigTo))
        {
          throw new XspException("Element to add the signature to cannot be " +
            "a descendant of an element being signed");
        }
      }

      // Create the reference list for signing
      IList<string> referenceList = new List<string>();
      foreach (XmlElement elementToSign in elementsToSign)
      {
        // Check if the element has an existing 'id' attribute
        string referenceId = null;
        IList<string> elemIdValues = GetIdValues(elementToSign);
        if (elemIdValues.Count == 0)
        {
          // There is no 'id' element on the attribute so create one and add it
          // Make sure reference starts with an alpha char - 'Malformed message' error if starts with number
          referenceId = "Id_" + Guid.NewGuid().ToString();          
          XmlUtils.AddIdAttribute(elementToSign, referenceId);
        }
        else
        {
          // Set the signature reference 'id' to the existing one
          referenceId = elemIdValues[0];
        }

        referenceList.Add(referenceId);
      }
      
      // Sign all the elements
      foreach (X509Certificate2 certificate in certificates)
      {
        // Create the signature for the element
        XmlElement signatureElem = XmlSecurityUtils.Sign(
          containerDoc, certificate, referenceList);
        
        // Append each created signature
        elementToAddToSigTo.AppendChild(signatureElem);
      }
    }
    
    /// <summary>
    /// Checks a signature.
    /// </summary>
    /// <param name="signatureElem">Signature element.</param>
    /// <param name="certificateVerifier">Certificate verifier.</param>
    public void Check(XmlElement signatureElem, 
      ICertificateVerifier certificateVerifier)
    {
      ArgumentUtils.CheckNotNull(signatureElem, "signatureElem");
      ArgumentUtils.CheckNotNull(certificateVerifier, "certificateVerifier");

      IList<XmlElement> signatureElems = new List<XmlElement>();
      signatureElems.Add(signatureElem);
      Check(signatureElems, certificateVerifier);
    }

    /// <summary>
    /// Checks a list of signatures.
    /// </summary>
    /// <param name="signatureElems">Signature elements.</param>
    /// <param name="certificateVerifier">Certificate verifier.</param>
    public void Check(IList<XmlElement> signatureElems, 
      ICertificateVerifier certificateVerifier)
    {
      ArgumentUtils.CheckNotNullNorEmpty(signatureElems, "signatureElems");
      ArgumentUtils.CheckNotNull(certificateVerifier, "certificateVerifier");
      
      // Check each signature
      foreach (XmlElement signatureElem in signatureElems)
      {
        if (!XmlUtils.CheckElement(signatureElem, SignatureElement,
          SignedXml.XmlDsigNamespaceUrl))
        {
          throw new XspException("Element is not a 'ds:Signature' element");
        }
            
        // Get the certificate from the 'KeyInfo' of the signature
        X509Certificate2 certificate = GetCertificate(signatureElem);

        // Verify the signature
        if (!XmlSecurityUtils.Verify(signatureElem, certificate))
        {
          throw new XspException("Could not validate the signature");
        }

        // Verify the certificate
        certificateVerifier.Verify(certificate);
      }
    }

    /// <summary>
    /// Gets the signing certificate.
    /// </summary>
    /// <param name="signatureElem">Signature element.</param>
    /// <returns>Certificate used to create the signature.</returns>
    public X509Certificate2 GetSigningCertificate(XmlElement signatureElem)
    {
      ArgumentUtils.CheckNotNull(signatureElem, "signatureElem");

      if (!XmlUtils.CheckElement(signatureElem, SignatureElement,
        SignedXml.XmlDsigNamespaceUrl))
      {
        throw new XspException("Element is not a 'ds:Signature' element");
      }
      
      // Get the certificate from the signature
      return GetCertificate(signatureElem);
    }
    
    /// <summary>
    /// Gets a list of digest values from a signature element.
    /// </summary>
    /// <param name="signatureElem">Signature element.</param>
    /// <returns>Map that contains reference Uri to digest values.</returns>
    public IDictionary<string, byte[]> GetDigestValues(XmlElement signatureElem)
    {
      ArgumentUtils.CheckNotNull(signatureElem, "signatureElem");

      if (!XmlUtils.CheckElement(signatureElem, SignatureElement, 
        SignedXml.XmlDsigNamespaceUrl))
      {
        throw new XspException("Element is not a 'ds:Signature' element");
      }
          
      // Load the signature
      SignedXml signedXml = new SignedXml(signatureElem.OwnerDocument);      
      signedXml.LoadXml(signatureElem);
      
      IDictionary<string, byte[]> digestValues = new Dictionary<string, byte[]>();

      // Iterate through each reference and add it to the map
      foreach (Reference signatureRef in signedXml.SignedInfo.References)
      {
        // Check the URI value is set
        if (signatureRef.Uri == null || signatureRef.Uri.Length == 0)
        {
          throw new XspException("Signature reference does not contain a URI");
        }
        // Check the digest value exists
        if (signatureRef.DigestValue == null)
        {
          throw new XspException("Signature reference does not contain a digest value");
        }
        digestValues.Add(signatureRef.Uri, signatureRef.DigestValue);
      }

      return digestValues;
    }
    
    /// <summary>
    /// Gets the certificate from the 'KeyInfo' of the signature.
    /// </summary>
    /// <param name="signatureElem">'ds:Signature' element.</param>
    /// <returns>Certificate</returns>
    /// <exception cref="XspException">Thrown if the certificate cannot be
    /// retrieved from the 'KeyInfo' of the signature.</exception>
    private static X509Certificate2 GetCertificate(XmlElement signatureElem)
    {
      SignedXml signedXml = new SignedXml(signatureElem.OwnerDocument);
      signedXml.LoadXml(signatureElem);

      KeyInfoX509Data keyInfoX509Data = null;
      IEnumerator keyInfoItems = signedXml.KeyInfo.GetEnumerator();
      while (keyInfoItems.MoveNext())
      {
        if (keyInfoItems.Current is KeyInfoX509Data)
        {
          keyInfoX509Data = (KeyInfoX509Data)keyInfoItems.Current;
          break;
        }
      }

      if (keyInfoX509Data == null)
      {
        throw new XspException(
          "Error getting they ds:KeyInfo: ds:X509 ds:KeyInfo was not found");
      }

      // Check if there are any certificates
      if (keyInfoX509Data.Certificates.Count == 0)
      {
        throw new XspException(
          "Error getting the certificate: no certificates within the ds:KeyInfo");
      }

      // Check if there is more than one certificate
      if (keyInfoX509Data.Certificates.Count > 1)
      {
        throw new XspException(
          "Error getting the certificate: More than one certificate found " +
          "within the ds:KeyInfo");
      }

      return (X509Certificate2)keyInfoX509Data.Certificates[0];
    }

    /// <summary>
    /// Gets any existing 'id' attribute values from an element.
    /// </summary>
    /// <param name="elementToSign">Element to be signed.</param>
    /// <returns>List of 'id' values otherwise an empty list.</returns>
    private static IList<string> GetIdValues(XmlElement elementToSign)
    {
      IList<string> idValues = new List<string>();
      XmlAttribute idAttr = elementToSign.Attributes[XmlIdLocalName];
      // Check if an 'id' attribute exists
      if (idAttr != null)
      {
        idValues.Add(idAttr.Value);
      }

      return idValues;
    }

  }
}
