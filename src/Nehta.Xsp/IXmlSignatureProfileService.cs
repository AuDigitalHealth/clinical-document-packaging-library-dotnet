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
  /// This interface provides functionality related to the 'Xml Signature Profile' 
  /// in the XML Secured Payload Profile document.
  /// </summary>
  public interface IXmlSignatureProfileService
  { 
    /// <summary>
    /// Signs data using Xml Signature which conforms to the Xml Signature Profile.
    /// </summary>
    /// <param name="elementToAddToSigTo">Element to add the signature to.</param>
    /// <param name="elementToSign">Element to sign.</param>
    /// <param name="certificate">Certificate to sign with.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Sign(XmlElement elementToAddToSigTo, XmlElement elementToSign,
      X509Certificate2 certificate);
    
    /// <summary>
    /// Signs data using Xml Signature which conforms to the Xml Signature Profile.
    /// </summary>
    /// <param name="elementToAddToSigTo">Element to add the signature to.</param>
    /// <param name="elementsToSign">List of elements to sign.</param>
    /// <param name="certificate">Certificate to sign with.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Sign(XmlElement elementToAddToSigTo, IList<XmlElement> elementsToSign,
      X509Certificate2 certificate);

    /// <summary>
    /// Signs data using Xml Signature which conforms to the Xml Signature Profile.
    /// </summary>
    /// <param name="elementToAddToSigTo">Element to add the signature to.</param>
    /// <param name="elementToSign">Element to sign.</param>
    /// <param name="certificates">List of certificates to sign with.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Sign(XmlElement elementToAddToSigTo, XmlElement elementToSign,
      X509Certificate2Collection certificates);

    /// <summary>
    /// Signs data using Xml Signature which conforms to the Xml Signature Profile.
    /// </summary>
    /// <param name="elementToAddToSigTo">Element to add the signature to.</param>
    /// <param name="elementsToSign">List of elements to sign.</param>
    /// <param name="certificates">List of certificates to sign with.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Sign(XmlElement elementToAddToSigTo, IList<XmlElement> elementsToSign,
      X509Certificate2Collection certificates);
    
    /// <summary>
    /// Checks the validity of a signature and verifies the certificate in 
    /// the signature using the certificate verifier callback.
    /// </summary>
    /// <param name="signatureElem">Signature element to check.</param>
    /// <param name="certificateVerifier">Certificate verifier callback.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Check(XmlElement signatureElem, ICertificateVerifier certificateVerifier);

    /// <summary>
    /// Checks the validity of a list of signatures and verifies the certificates in 
    /// the signatures using the certificate verifier callback.
    /// </summary>
    /// <param name="signatureElems"></param>
    /// <param name="certificateVerifier"></param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Check(IList<XmlElement> signatureElems, 
      ICertificateVerifier certificateVerifier);

    /// <summary>
    /// Gets the signing certificate from the 'KeyInfo' of the signature.
    /// </summary>
    /// <param name="signatureElem">'ds:Signature' element.</param>
    /// <returns>Extracted certificate.</returns>
    X509Certificate2 GetSigningCertificate(XmlElement signatureElem);

    /// <summary>
    /// Gets the list of IDs and their corresponding digest values from
    /// the signature.
    /// </summary>
    /// <param name="signatureElem">'ds:Signature' element.</param>
    /// <returns>Map containing the signing ID and corresponding digest value.</returns>
    IDictionary<string, byte[]> GetDigestValues(XmlElement signatureElem);    
  }
}
