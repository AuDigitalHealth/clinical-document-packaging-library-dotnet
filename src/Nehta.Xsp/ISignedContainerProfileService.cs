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
using System.Collections.Generic;


namespace Nehta.Xsp
{
  /// <summary>
  /// This interface provides functionality related to the 'Signed Container Profile' 
  /// in the XML Secured Payload Profile document.
  /// </summary>
  public interface ISignedContainerProfileService
  {
    /// <summary>
    /// Signs Xml data with a certificate and creates a signed payload container.
    /// </summary>
    /// <param name="payloadDoc">Document to sign.</param>
    /// <param name="certificate">Certificate used to sign with.</param>
    /// <returns>Signed container.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    XmlDocument Create(XmlDocument payloadDoc, X509Certificate2 certificate);

    /// <summary>
    /// Signs Xml data with multiple certificates and creates a signed payload container.
    /// </summary>
    /// <param name="payloadDoc">Document to sign.</param>
    /// <param name="certificates">List of certificates used to sign with.</param>
    /// <returns>Signed container.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    XmlDocument Create(XmlDocument payloadDoc, X509Certificate2Collection certificates);
    
    /// <summary>
    /// Extracts the payload from the signed payload container.
    /// </summary>
    /// <param name="containerDoc">Signed payload container.</param>
    /// <returns>Payload.</returns>
    /// <exception cref="XspException">Thrown on error.</exception>
    XmlDocument GetData(XmlDocument containerDoc);

    /// <summary>
    /// Checks the signatures in a signed payload container and also verifies the 
    /// certificates with the certificate verifier callback.
    /// </summary>
    /// <param name="containerDoc">Signed payload container.</param>
    /// <param name="certificateVerifier">Certificate verifier.</param>
    /// <exception cref="XspException">Thrown on error.</exception>
    void Check(XmlDocument containerDoc, ICertificateVerifier certificateVerifier);
        
    /// <summary>
    /// Returns a list of certificates from the signed container.
    /// </summary>
    /// <param name="containerDoc">Signed container.</param>
    /// <returns>List of certificates.</returns>     
    IList<X509Certificate2> GetSigningCertificates(XmlDocument containerDoc);
    
    /// <summary>
    /// Returns a list of digest values from the signatures within the
    /// container.
    /// </summary>
    /// <param name="containerDoc">Signed container.</param>
    /// <returns>List of digest values.</returns>
    IList<byte[]> GetDigestValues(XmlDocument containerDoc);
  }
}
