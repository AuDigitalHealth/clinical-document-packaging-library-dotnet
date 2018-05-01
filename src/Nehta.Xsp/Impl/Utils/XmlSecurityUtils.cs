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
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;


namespace Nehta.Xsp.Impl.Utils
{
  /// <summary>
  /// Utility class that contains Xml security related methods.
  /// </summary>
  public static class XmlSecurityUtils
  {
    /// <summary>
    /// Signs a list of references using a certificate and returns
    /// the signature element.
    /// </summary>
    /// <param name="xmlDoc">Xml document that contains the references. 
    /// Cannot be null.</param>
    /// <param name="signCert">Certificate used for signing that contains a 
    /// private key. The certificate and contained private key cannot be null.</param>
    /// <param name="references">List of references to sign. Cannot be null.</param>
    /// <returns>'ds:Signature' element</returns>
    public static XmlElement Sign(XmlDocument xmlDoc, X509Certificate2 signCert,
      IList<string> references)
    {
      // Create the signature object using the document as the context
      SignedXml signedXml = new SignedXml(xmlDoc);
      signedXml.SigningKey = signCert.PrivateKey;

      // Specify the canonicalization method
      signedXml.Signature.SignedInfo.CanonicalizationMethod =
        SignedXml.XmlDsigExcC14NTransformUrl;

      // Specify the signature method
      signedXml.Signature.SignedInfo.SignatureMethod =
        SignedXml.XmlDsigRSASHA1Url;

      // Add all the signing references
      foreach (string signReferenceId in references)
      {
        Reference reference = new Reference();
        reference.Uri = "#" + signReferenceId;
        reference.DigestMethod = SignedXml.XmlDsigSHA1Url;

        // Add the reference transform
        XmlDsigExcC14NTransform transform = new XmlDsigExcC14NTransform();
        reference.AddTransform(transform);

        // Add the reference to the signature
        signedXml.AddReference(reference);
      }

      // Calculate the signature
      signedXml.ComputeSignature();

      // Add the key information to the signature 
      KeyInfo keyInfo = new KeyInfo();
      KeyInfoX509Data kixd = new KeyInfoX509Data();
      kixd.AddCertificate(signCert);
      keyInfo.AddClause(kixd);
      signedXml.KeyInfo = keyInfo;

      // Return the signature
      return signedXml.GetXml();
    }

    /// <summary>
    /// Verifies an Xml signature using a certificate.
    /// </summary>
    /// <param name="signatureElem">'ds:Signature' element. Cannot be null.</param>
    /// <param name="certificate">Certificate used to verify. Cannot be null.</param>
    /// <returns>True when the signature verifies otherwise false.</returns>
    public static bool Verify(XmlElement signatureElem, 
      X509Certificate2 certificate)
    {
      // Load the signature using the document as the context
      XmlDocument ownerDoc = signatureElem.OwnerDocument;
      
      // Use NehtaSignedXml override to avoid the 'Malformed Reference Element' when
      // Reference id starts with a number which a MS16-035 Windows Security Update
      // that causes this behaviour. 8th March 2016.
      NehtaSignedXml signedXml = new NehtaSignedXml(ownerDoc);
      signedXml.LoadXml(signatureElem);

      // Check the signature is valid using the public key from the certificate
      return signedXml.CheckSignature(certificate.PublicKey.Key);
    }

    /// <summary>
    /// Encrypts data and returns the 'EncryptedData' object.
    /// </summary>
    /// <param name="symmetricKey">Symmetric key used for encryption. 
    /// Cannot be null.</param>
    /// <param name="elementToEncrypt">Element to encrypt. Cannot be 
    /// null.</param>
    /// <param name="referenceId">ID used to reference the key used to 
    /// encrypt the data. Cannot be null.</param>
    /// <returns>'EncryptedData' object.</returns>
    public static EncryptedData Encrypt(SymmetricAlgorithm symmetricKey,
      XmlElement elementToEncrypt, string referenceId)
    {
      EncryptedXml encryptedXml = new EncryptedXml(
        elementToEncrypt.OwnerDocument);

      // Encrypt the data using the session key
      byte[] bytes = encryptedXml.EncryptData(
        elementToEncrypt, symmetricKey, false);

      EncryptedData encryptedData = new EncryptedData();
      
      // Set the encryption type and method
      encryptedData.Type = EncryptedXml.XmlEncElementUrl;
      encryptedData.EncryptionMethod =
        new EncryptionMethod(EncryptedXml.XmlEncAES256Url);

      // Set the 'Id' attribute to allow referencing from an encrypted key
      encryptedData.Id = referenceId;
      encryptedData.CipherData.CipherValue = bytes;

      return encryptedData;
    }

    /// <summary>
    /// Decrypts an encrypted data element using a session key.
    /// </summary>
    /// <param name="encryptedDataElem">'xenc:EncryptedData' element to 
    /// decrypt. Cannot be null.</param>
    /// <param name="sessionKey">Session key for decryption. Cannot be null.
    /// </param>
    /// <returns>Decrypted data.</returns>
    public static byte[] Decrypt(XmlElement encryptedDataElem, 
      SymmetricAlgorithm sessionKey)     
    {
      EncryptedXml encryptedXml =
        new EncryptedXml(encryptedDataElem.OwnerDocument);

      // Load the encrypted data
      EncryptedData encryptedData = new EncryptedData();
      encryptedData.LoadXml(encryptedDataElem);

      // Perform the decryption
      return encryptedXml.DecryptData(encryptedData, sessionKey);
    }
    
    /// <summary>
    /// Creates an encrypted key.
    /// </summary>
    /// <param name="symmetricKey">Symmetric key that is to be encrypted.
    /// Cannot be null.</param>
    /// <param name="encryptCertificate">Certificate that will be 
    /// used to encrypt the symmetric key. Cannot be null.</param>
    /// <param name="referenceIds">List of IDs which reference the 
    /// 'xenc:EncryptedData' elements the key was used to encrypt.
    /// Cannot be null or empty.</param>
    /// <returns>'EncryptedKey' object.</returns>
    public static EncryptedKey CreateEncryptedKey(SymmetricAlgorithm symmetricKey,
      X509Certificate2 encryptCertificate, IList<string> referenceIds)
    {
      // Encrypt the session key using the public key 
      byte[] encryptedKeyData =
        EncryptedXml.EncryptKey(symmetricKey.Key, 
          (RSA)encryptCertificate.PublicKey.Key, false);

      // Create the encrypted key
      EncryptedKey encryptedKey = new EncryptedKey();
      encryptedKey.CipherData = new CipherData(encryptedKeyData);
      encryptedKey.EncryptionMethod =
        new EncryptionMethod(EncryptedXml.XmlEncRSA15Url);

      // Set the key information for the encrypted key
      KeyInfoX509Data xd = new KeyInfoX509Data();
      xd.AddSubjectKeyId(CertificateUtils.GetSubjectKeyIdentifier(
        encryptCertificate).SubjectKeyIdentifier);
      encryptedKey.KeyInfo.AddClause(xd);

      // Add a data reference for each identifier
      foreach (string referenceId in referenceIds)
      {
        DataReference dataReference = new DataReference("#" + referenceId);
        encryptedKey.ReferenceList.Add(dataReference);
      }

      return encryptedKey;
    }
    
    /// <summary>
    /// Decrypts an encrypted key.
    /// </summary>
    /// <param name="encryptedKey">Encrypted key to decrypt. Cannot be null.
    /// </param>
    /// <param name="privateKey">Private key that will be used to decrypt 
    /// the encrypted key. Cannot be null.</param>
    /// <returns>Decrypted key.</returns>
    public static byte[] DecryptEncryptedKey(EncryptedKey encryptedKey, 
      AsymmetricAlgorithm privateKey)
    {
      return EncryptedXml.DecryptKey(encryptedKey.CipherData.CipherValue,
        (RSA)privateKey, false);  
    }
  }
}
