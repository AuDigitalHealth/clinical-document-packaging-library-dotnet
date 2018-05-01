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
using System.Xml;
using System.Security.Cryptography.X509Certificates;


namespace Nehta.Xsp
{
  /// <summary>
  /// This interface provides functionality related to the 'Encrypted Container Profile' 
  /// in the XML Secured Payload Profile document.
  /// </summary>
  public interface IEncryptedContainerProfileService
  {
    /// <summary>
    /// Encrypts Xml data using one certificate and and creates an encrypted 
    /// payload container.
    /// </summary>
    /// <param name="payloadDoc">Payload to encrypt.</param>
    /// <param name="certificate">Certificate to encrypt with.</param>
    /// <returns>Encrypted container Xml document.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    XmlDocument Create(XmlDocument payloadDoc, X509Certificate2 certificate);

    /// <summary>
    /// Encrypts Xml data using multiple certificates and creates an encrypted 
    /// payload container.
    /// </summary>
    /// <param name="payloadDoc">Payload to encrypt.</param>
    /// <param name="certificates">Certificates to encrypt with.</param>
    /// <returns>Encrypted container Xml document.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    XmlDocument Create(XmlDocument payloadDoc, X509Certificate2Collection certificates);

    /// <summary>
    /// Encrypts Xml data using multiple certificates and creates an encrypted 
    /// payload container.
    /// </summary>
    /// <param name="payloadDoc">Payload to encrypt.</param>
    /// <param name="certificates">Certificates to encrypt with.</param>
    /// <param name="cipherKey">The cipher to use when encrypting the payload.</param>
    /// <returns>Encrypted container Xml document.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    XmlDocument Create(XmlDocument payloadDoc, X509Certificate2Collection certificates, byte[] cipherKey);

    /// <summary>
    /// Decrypts an encrypted container document returning the payload.
    /// </summary>
    /// <param name="encryptedPayloadDoc">Encrypted container to decrypt.</param>
    /// <param name="certificate">Certificate to decrypt with.</param>
    /// <returns>Decrypted payload.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    XmlDocument GetData(XmlDocument encryptedPayloadDoc, X509Certificate2 certificate);

    /// <summary>
    /// Decrypts an encrypted container Xml element, returning the payload.
    /// </summary>
    /// <param name="encryptedPayloadElem">Encrypted container to decrypt.</param>
    /// <param name="certificate">Certificate to decrypt with.</param>
    /// <returns>Decrypted payload.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    XmlDocument GetData(XmlElement encryptedPayloadElem, X509Certificate2 certificate);

    /// <summary>
    /// Decrypts an encrypted container document returning the payload.
    /// </summary>
    /// <param name="encryptedPayloadDoc">Encrypted container to decrypt.</param>
    /// <param name="cipherKey">The cipher key to use to decrypt the document.</param>
    /// <returns>Decrypted payload.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    XmlDocument GetData(XmlDocument encryptedPayloadDoc, byte[] cipherKey);
  }
}
