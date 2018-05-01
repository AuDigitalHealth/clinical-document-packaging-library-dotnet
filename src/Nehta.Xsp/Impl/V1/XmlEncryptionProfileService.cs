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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

using Nehta.Xsp.Impl.Utils;
using Nehta.Common.Utils;

namespace Nehta.Xsp.Impl.V1
{
  /// <summary>
  /// Implementation of 'IEncryptedProfileService' interface that supports
  /// XML Secured Payload Profile.
  /// </summary>
  public class XmlEncryptionProfileService : IXmlEncryptionProfileService
  {
    private const string EncryptedKeyTag = "EncryptedKey";
    
    private const string EncryptedDataTag = "EncryptedData";
    

    /// <summary>
    /// Encrypts an element.
    /// </summary>
    /// <param name="elementToAddEncKeysTo">Element to add the encrypted key to.</param>
    /// <param name="elementToEncrypt">Element to encrypt.</param>
    /// <param name="certificate">Certificate to use for encryption.</param>  
    public void Encrypt(XmlElement elementToAddEncKeysTo, 
      XmlElement elementToEncrypt, X509Certificate2 certificate)
    {
      ArgumentUtils.CheckNotNull(elementToAddEncKeysTo, "elementToAddEncKeysTo");
      ArgumentUtils.CheckNotNull(elementToEncrypt, "elementToEncrypt");
      ArgumentUtils.CheckNotNull(certificate, "certificate");

      X509Certificate2Collection certificates = 
        new X509Certificate2Collection(certificate);
      IList<XmlElement> elementsToEncrypt = new List<XmlElement>();
      elementsToEncrypt.Add(elementToEncrypt);
      
      Encrypt(elementToAddEncKeysTo, elementsToEncrypt, certificates);
    }

    /// <summary>
    /// Encrypts a list of elements.
    /// </summary>
    /// <param name="elementToAddEncKeysTo"></param>
    /// <param name="elementsToEncrypt"></param>
    /// <param name="certificate"></param>
    public void Encrypt(XmlElement elementToAddEncKeysTo, 
      IList<XmlElement> elementsToEncrypt, X509Certificate2 certificate)
    {
      ArgumentUtils.CheckNotNull(elementToAddEncKeysTo, "elementToAddEncKeysTo");
      ArgumentUtils.CheckNotNullNorEmpty(elementsToEncrypt, "elementsToEncrypt");
      ArgumentUtils.CheckNotNull(certificate, "certificate");

      X509Certificate2Collection certificates = new X509Certificate2Collection();
      certificates.Add(certificate);

      Encrypt(elementToAddEncKeysTo, elementsToEncrypt, certificates);
    }

    /// <summary>
    /// Encrypts an element.
    /// </summary>
    /// <param name="elementToAddEncKeysTo"></param>
    /// <param name="elementToEncrypt"></param>
    /// <param name="certificates"></param>
    public void Encrypt(XmlElement elementToAddEncKeysTo, XmlElement elementToEncrypt, 
      X509Certificate2Collection certificates)
    {
      ArgumentUtils.CheckNotNull(elementToAddEncKeysTo, "elementToAddEncKeysTo");
      ArgumentUtils.CheckNotNull(elementToEncrypt, "elementToEncrypt");
      CertificateUtils.CheckNotNullOrEmpty(certificates, "certificates");
      
      IList<XmlElement> elementsToEncrypt = new List<XmlElement>();
      elementsToEncrypt.Add(elementToEncrypt);

      Encrypt(elementToAddEncKeysTo, elementsToEncrypt, certificates);
    }

    /// <summary>
    /// Encrypts a list of XML elements.
    /// </summary>
    /// <param name="elementToAddEncKeysTo"></param>
    /// <param name="elementsToEncrypt"></param>
    /// <param name="certificates"></param>
    public void Encrypt(XmlElement elementToAddEncKeysTo,
      IList<XmlElement> elementsToEncrypt,
      X509Certificate2Collection certificates)
    {
        Encrypt(elementToAddEncKeysTo, elementsToEncrypt, certificates, null);
    }

    /// <summary>
    /// Encrypts a list of elements.
    /// </summary>
    /// <param name="elementToAddEncKeysTo"></param>
    /// <param name="elementsToEncrypt"></param>
    /// <param name="certificates"></param>
    /// <param name="cipherKey"></param>
    public void Encrypt(XmlElement elementToAddEncKeysTo, 
      IList<XmlElement> elementsToEncrypt,
      X509Certificate2Collection certificates, byte[] cipherKey)
    {
      ArgumentUtils.CheckNotNull(elementToAddEncKeysTo, "elementToAddEncKeysTo");
      ArgumentUtils.CheckNotNullNorEmpty(elementsToEncrypt, "elementsToEncrypt");
      CertificateUtils.CheckNotNullOrEmpty(certificates, "certificates");

      // Check all the elements to encrypt are not the same as the element
      // to add the keys to and check they belong to the same document.
      foreach (XmlElement elementToEncrypt in elementsToEncrypt)
      {
        if (elementToEncrypt == elementToAddEncKeysTo)
        {
          throw new XspException(
            "Cannot add keys to an element that is being encrypted");
        }

        if (elementToAddEncKeysTo.OwnerDocument != 
          elementToEncrypt.OwnerDocument)
        {
          throw new XspException(
            "Elements to encrypt must belong to the same document as the " + 
            "keys element");
        }

        if (XmlUtils.IsDescendant(elementToEncrypt, elementToAddEncKeysTo))
        {
          throw new XspException(
            "Element the keys are added to cannot be a child element of an " +
            "element to encrypt");
        }
      }

      // Get the container document
      XmlDocument containerDoc = elementToAddEncKeysTo.OwnerDocument;

      // Create a random session key
      RijndaelManaged sessionKey = new RijndaelManaged();
      sessionKey.KeySize = 256;

      if (cipherKey != null) sessionKey.Key = cipherKey;

      IList<string> referenceIdList = new List<string>();     
      foreach (XmlElement elementToEncrypt in elementsToEncrypt)
      {
        // Generate a unique reference identifier
        string referenceId = "_" + Guid.NewGuid().ToString();

        // Add it to the reference list
        referenceIdList.Add(referenceId);

        // Create the encrypted data
        EncryptedData encryptedData = XmlSecurityUtils.Encrypt(
          sessionKey, elementToEncrypt, referenceId);

        // Replace the original element with the encrypted data
        EncryptedXml.ReplaceElement(elementToEncrypt, encryptedData, false);
      }

      foreach (X509Certificate2 certificate in certificates)
      {
        // Create the encrypted key
        EncryptedKey encryptedKey = XmlSecurityUtils.CreateEncryptedKey(
          sessionKey, certificate, referenceIdList);

        // Import the encrypted key element into the container document
        XmlNode encryptedKeyElem = 
          containerDoc.ImportNode(encryptedKey.GetXml(), true);

        elementToAddEncKeysTo.AppendChild(encryptedKeyElem);
      }
    }

    /// <summary>
    /// Decrypts an element.
    /// </summary>
    /// <param name="encryptedKeyElem"></param>
    /// <param name="encryptedDataElem"></param>
    /// <param name="certificate"></param>
    public void Decrypt(XmlElement encryptedKeyElem, XmlElement encryptedDataElem, 
      X509Certificate2 certificate)
    {
      ArgumentUtils.CheckNotNull(encryptedKeyElem, "encryptedKeyElem");
      ArgumentUtils.CheckNotNull(encryptedDataElem, "encryptedDataElem");
      ArgumentUtils.CheckNotNull(certificate, "certificate");

      IList<XmlElement> encryptedKeyElems = new List<XmlElement>();
      encryptedKeyElems.Add(encryptedKeyElem);
      IList<XmlElement> encryptedDataElems = new List<XmlElement>();
      encryptedDataElems.Add(encryptedDataElem);
      Decrypt(encryptedKeyElems, encryptedDataElems, certificate);
    }

    /// <summary>
    /// Decrypts an element.
    /// </summary>
    /// <param name="encryptedKeyElems"></param>
    /// <param name="encryptedDataElem"></param>
    /// <param name="certificate"></param>
    public void Decrypt(IList<XmlElement> encryptedKeyElems, 
      XmlElement encryptedDataElem, X509Certificate2 certificate)
    {
      ArgumentUtils.CheckNotNullNorEmpty(encryptedKeyElems, "encryptedKeyElems");
      ArgumentUtils.CheckNotNull(encryptedDataElem, "encryptedDataElem");
      ArgumentUtils.CheckNotNull(certificate, "certificate");

      IList<XmlElement> encryptedDataElems = new List<XmlElement>();
      encryptedDataElems.Add(encryptedDataElem);
      Decrypt(encryptedKeyElems, encryptedDataElems, certificate);
    }

    /// <summary>
    /// Decrypts a list of elements.
    /// </summary>
    /// <param name="encryptedKeyElem"></param>
    /// <param name="encryptedDataElems"></param>
    /// <param name="certificate"></param>
    public void Decrypt(XmlElement encryptedKeyElem, 
      IList<XmlElement> encryptedDataElems, X509Certificate2 certificate)
    {
      ArgumentUtils.CheckNotNull(encryptedKeyElem, "encryptedKeyElem");
      ArgumentUtils.CheckNotNullNorEmpty(encryptedDataElems, 
        "encryptedDataElems");
      ArgumentUtils.CheckNotNull(certificate, "certificate");

      IList<XmlElement> encryptedKeyElems = new List<XmlElement>();
      encryptedKeyElems.Add(encryptedKeyElem);
      Decrypt(encryptedKeyElems, encryptedDataElems, certificate);
    }

    /// <summary>
    /// Decrypts a list of elements.
    /// </summary>
    /// <param name="encryptedKeyElems">List of encrypted key elements.</param>
    /// <param name="encryptedDataElems">List of encrypted data elements.</param>
    /// <param name="certificate">Certificate to use for key decryption.</param>
    public void Decrypt(IList<XmlElement> encryptedKeyElems, 
      IList<XmlElement> encryptedDataElems, X509Certificate2 certificate)
    {
      ArgumentUtils.CheckNotNullNorEmpty(encryptedKeyElems, 
        "encryptedKeyElems");
      ArgumentUtils.CheckNotNullNorEmpty(encryptedDataElems, 
        "encryptedDataElems");
      ArgumentUtils.CheckNotNull(certificate, "certificate");

      // Check the certificate has a private key
      if (certificate.PrivateKey == null)
      {
        throw new XspException("Certificate with subject '" + 
          certificate.Subject + "' does not contain a private key");
      }

      XmlDocument containerDoc = encryptedKeyElems[0].OwnerDocument;

      // Check the 'encryptedKeyElems' elements
      foreach (XmlElement encryptedKeyElem in encryptedKeyElems)
      {
        // Check they are all 'xenc:EncryptedKey' elements
        if (!XmlUtils.CheckElement(encryptedKeyElem, EncryptedKeyTag,
          EncryptedXml.XmlEncNamespaceUrl))
        {
          throw new XspException("Element within the keys list is not " + 
            "an 'xenc:EncryptedKey'");        
        }

        // Check they all belong to the same document
        if (encryptedKeyElem.OwnerDocument != containerDoc)
        {
          throw new XspException("All 'xenc:EncryptedKey' elements " +
            "must belong to the same document");
        }
      }

      // Check the 'encryptedDataElems' elements
      foreach (XmlElement encryptedDataElem in encryptedDataElems)
      {
        // Check they are all 'xenc:EncryptedData' elements
        if (!XmlUtils.CheckElement(encryptedDataElem, EncryptedDataTag,
          EncryptedXml.XmlEncNamespaceUrl))
        {
          throw new XspException("Element within the encrypted data list is " +
            "not an 'xenc:EncryptedData' element.");        
        }
        
        // Check they all belong to the same document
        if (encryptedDataElem.OwnerDocument != containerDoc)
        {
          throw new XspException("All 'xenc:EncryptedData' elements " +
            "must belong to the same document");        
        }
      }
      
      // Attempt to find the matching encrypted key for the certificate
      EncryptedKey encryptedKey = null;
      foreach (XmlElement encryptedKeyElem in encryptedKeyElems)
      {
        EncryptedKey currentEncryptedKey = new EncryptedKey();
        currentEncryptedKey.LoadXml(encryptedKeyElem);
        
        // Check if the subject key identifier specified within the 
        // 'KeyInfo' of the encrypted key matches the certificate
        if (MatchesCertificate(currentEncryptedKey, certificate))
        {
          encryptedKey = currentEncryptedKey;
          break;
        }
      }
      
      // Check if a key was found
      if (encryptedKey == null)
      {
        throw new KeyMismatchException(
          "Could not find a matching encrypted key for certificate '" + 
          certificate.Subject + "'.");
      }

      // Decrypt the encrypted key
      RijndaelManaged sessionKey = new RijndaelManaged();
      sessionKey.Key = XmlSecurityUtils.DecryptEncryptedKey(
        encryptedKey, certificate.PrivateKey);

      // Decrypt each of the encrypted data elements using the decrypted key
      foreach (XmlElement encryptedDataElem in encryptedDataElems)
      {
        // Decrypt the data
        byte[] decryptedData = XmlSecurityUtils.Decrypt(
          encryptedDataElem, sessionKey);

        // Replace the encrypted data with the decrypted data within the container
        EncryptedXml encryptedXml = new EncryptedXml(containerDoc);
        encryptedXml.ReplaceData(encryptedDataElem, decryptedData);
      }
    }

    /// <summary>
    /// Decrypts a list of elements using the cipher key.
    /// </summary>
    /// <param name="encryptedDataElems">Elements to decrypt.</param>
    /// <param name="cipherKey">Cipher key.</param>
    public void Decrypt(IList<XmlElement> encryptedDataElems, byte[] cipherKey)
    {
        ArgumentUtils.CheckNotNullNorEmpty(encryptedDataElems,  "encryptedDataElems");
        ArgumentUtils.CheckNotNull(cipherKey, "cipherKey");

        // Decrypt the encrypted key
        RijndaelManaged sessionKey = new RijndaelManaged();
        sessionKey.Key = cipherKey;

        // Decrypt each of the encrypted data elements using the decrypted key
        foreach (XmlElement encryptedDataElem in encryptedDataElems)
        {
            // Decrypt the data
            byte[] decryptedData = XmlSecurityUtils.Decrypt(encryptedDataElem, sessionKey);

            XmlDocument containerDoc = encryptedDataElem.OwnerDocument;

            // Replace the encrypted data with the decrypted data within the container
            EncryptedXml encryptedXml = new EncryptedXml(containerDoc);
            encryptedXml.ReplaceData(encryptedDataElem, decryptedData);
        }
    }

    /// <summary>
    /// Decrypts an element using the cipher key.
    /// </summary>
    /// <param name="encryptedDataElem">Element to decrypt.</param>
    /// <param name="cipherKey">Cipher key.</param>
    public void Decrypt(XmlElement encryptedDataElem, byte[] cipherKey)
    {
        ArgumentUtils.CheckNotNull(encryptedDataElem, "encryptedDataElem");
        ArgumentUtils.CheckNotNull(cipherKey, "cipherKey");

        IList<XmlElement> dataElems = new List<XmlElement>();
        dataElems.Add(encryptedDataElem);

        Decrypt(dataElems, cipherKey);
    }

    /// <summary>
    /// Returns true when the encrypted key matches the certificate.
    /// </summary>
    /// <param name="encryptedKey">Encrypted key to check.</param>
    /// <param name="certificate">Certificate to check.</param>
    /// <returns>True when they match otherwise false.</returns>
    private static bool MatchesCertificate(EncryptedKey encryptedKey, 
      X509Certificate2 certificate)
    {
      // Get the subject key identifier from the encrypted key
      X509SubjectKeyIdentifierExtension encryptedKeySki =
        new X509SubjectKeyIdentifierExtension(
          GetEncryptedKeySki(encryptedKey), false);

      // Get the subject key identifier from the certificate
      X509SubjectKeyIdentifierExtension certificateSki = 
        CertificateUtils.GetSubjectKeyIdentifier(certificate);

      // Check if the subject key identifiers match
      return encryptedKeySki.SubjectKeyIdentifier == 
        certificateSki.SubjectKeyIdentifier;
    }
    
    /// <summary>
    /// Extracts the subject key identifier from the encrypted key.
    /// </summary>
    /// <param name="encryptedKey">Encrypted key to extract the 
    /// subject key identifier from.</param>
    /// <returns>Subject key identifier otherwise throws an exception.
    /// </returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    private static byte[] GetEncryptedKeySki(EncryptedKey encryptedKey)
    {
      // Find the 'ds:X509Data' element
      KeyInfoX509Data keyInfoX509 = null;
      IEnumerator keyInfoItems = encryptedKey.KeyInfo.GetEnumerator();
      while (keyInfoItems.MoveNext())
      {
        if (keyInfoItems.Current is KeyInfoX509Data)
        {
          keyInfoX509 = (KeyInfoX509Data)keyInfoItems.Current;
          break;
        }
      }
      
      // Check if the element was found
      if (keyInfoX509 == null)
      {
        throw new XspException(
          "Error getting the KeyInfoX509Data object: KeyInfoX509Data was " +
          "not found in the encrypted key.");
      }
      
      // Check if there are any subject key identifiers specified
      if (keyInfoX509.SubjectKeyIds.Count == 0)
      {
        throw new XspException(
          "Error matching the encrypted key: no subject key identifier found");
      }
      
      // Check if there is more than one specified
      if (keyInfoX509.SubjectKeyIds.Count > 1)
      {
        throw new XspException(
          "Error matching the encrypted key: more than one subject " + 
          "key identifier found");
      }

      return (byte[])keyInfoX509.SubjectKeyIds[0];
    }

  }
}
