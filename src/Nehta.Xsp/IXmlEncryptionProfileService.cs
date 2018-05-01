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
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Nehta.Xsp
{
  /// <summary>
  /// This interface provides functionality related to the 'Xml Encryption Profile' 
  /// in the XML Secured Payload Profile document.
  /// </summary>
  public interface IXmlEncryptionProfileService
  {
    /// <summary>
    /// Encrypts data using Xml Encryption which conforms to the Xml Encryption Profile.
    /// </summary>
    /// <param name="elementToAddEncKeysTo">Xml element to add the encrypted keys to.
    /// </param>
    /// <param name="elementToEncrypt">Xml element to encrypt.</param>
    /// <param name="certificate">Certificate to encrypt with.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Encrypt(XmlElement elementToAddEncKeysTo, XmlElement elementToEncrypt,
      X509Certificate2 certificate);

    /// <summary>
    /// Encrypts data using Xml Encryption which conforms to the Xml Encryption Profile.
    /// </summary>
    /// <param name="elementToAddEncKeysTo">Xml element to add the encrypted keys to.
    /// </param>
    /// <param name="elementsToEncrypt">List of Xml elements to encrypt.</param>
    /// <param name="certificate">Certificate to encrypt with.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Encrypt(XmlElement elementToAddEncKeysTo, IList<XmlElement> elementsToEncrypt,
      X509Certificate2 certificate);

    /// <summary>
    /// Encrypts data using Xml Encryption which conforms to the Xml Encryption Profile.
    /// </summary>
    /// <param name="elementToAddEncKeysTo">Xml element to add the encrypted keys to.
    /// </param>
    /// <param name="elementToEncrypt">Xml element to encrypt.</param>
    /// <param name="certificates">List of certificates to encrypt with.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Encrypt(XmlElement elementToAddEncKeysTo, XmlElement elementToEncrypt,
      X509Certificate2Collection certificates);

    /// <summary>
    /// Encrypts data using Xml Encryption which conforms to the Xml Encryption Profile.
    /// </summary>
    /// <param name="elementToAddEncKeysTo">Xml element to add the encrypted keys to.
    /// </param>
    /// <param name="elementsToEncrypt">List of elements to encrypt.</param>
    /// <param name="certificates">List of certificates to encrypt with.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Encrypt(XmlElement elementToAddEncKeysTo, IList<XmlElement> elementsToEncrypt,
      X509Certificate2Collection certificates);

    /// <summary>
    /// Encrypts a list of elements.
    /// </summary>
    /// <param name="elementToAddEncKeysTo">Element to add encrypted keys to.</param>
    /// <param name="elementsToEncrypt">Elements to encrypt.</param>
    /// <param name="certificates">Certificates used for encrypting the cipher key.</param>
    /// <param name="cipherKey">Cipher key for encryption.</param>
    void Encrypt(XmlElement elementToAddEncKeysTo, IList<XmlElement> elementsToEncrypt,
        X509Certificate2Collection certificates, byte[] cipherKey);

    /// <summary>
    /// Decrypts data that has been encrypted using XML Encryption.
    /// </summary>
    /// <param name="encryptedKeyElem">Encrypted key element.</param>
    /// <param name="encryptedDataElem">Encrypted data element.</param>
    /// <param name="certificate">Certificate to decrypt with.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Decrypt(XmlElement encryptedKeyElem, XmlElement encryptedDataElem,
      X509Certificate2 certificate);

    /// <summary>
    /// Decrypts data that has been encrypted using XML Encryption.
    /// </summary>
    /// <param name="encryptedKeyElems">List of encrypted key elements.</param>
    /// <param name="encryptedDataElem">Encrypted data element.</param>
    /// <param name="certificate">Certificate to decrypt with.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Decrypt(IList<XmlElement> encryptedKeyElems, XmlElement encryptedDataElem,
      X509Certificate2 certificate);

    /// <summary>
    /// Decrypts data that has been encrypted using XML Encryption.
    /// </summary>
    /// <param name="encryptedKeyElem">Encrypted key element.</param>
    /// <param name="encryptedDataElems">List of encrypted data elements.</param>
    /// <param name="certificate">Certificate to decrypt with.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Decrypt(XmlElement encryptedKeyElem, IList<XmlElement> encryptedDataElems,
      X509Certificate2 certificate);

    /// <summary>
    /// Decrypts data that has been encrypted using XML Encryption.
    /// </summary>
    /// <param name="encryptedKeyElems">List of encrypted key elements.</param>
    /// <param name="encryptedDataElems">List of encrypted data elements.</param>
    /// <param name="certificate">Certificate to decrypt with.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Decrypt(IList<XmlElement> encryptedKeyElems, IList<XmlElement> encryptedDataElems,
      X509Certificate2 certificate);

    /// <summary>
    /// Decrypts a list of elements using the cipher key.
    /// </summary>
    /// <param name="encryptedDataElems">Elements to decrypt.</param>
    /// <param name="cipherKey">Cipher key.</param>
    void Decrypt(IList<XmlElement> encryptedDataElems, byte[] cipherKey);

    /// <summary>
    /// Decrypts an element using the cipher key.
    /// </summary>
    /// <param name="encryptedDataElem">Element to decrypt.</param>
    /// <param name="cipherKey">Cipher key.</param>
    void Decrypt(XmlElement encryptedDataElem, byte[] cipherKey);
  }
}
