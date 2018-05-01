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
using System.Security.Cryptography.X509Certificates;


namespace Nehta.Xsp
{
    /// <summary>
    /// Callback interface used by the 'ISignedProfileService' and the 
    /// 'ISignedContainerProfileService' to verify certificates in an Xml signature. 
    /// Verification of a certificate can include CRL checking, expiry and trust 
    /// chain checking.
    /// </summary>
    public interface ICertificateVerifier
    {
        /// <summary>
        /// Verifies that a given certificate is valid. When a certificate is invalid
        /// a 'CertificateValidationException' is thrown.
        /// </summary>
        /// <param name="certificate">Certificate to check. Cannot be null.</param>
        /// <exception cref="CertificateVerificationException">Thrown when the certificate 
        /// could not be verified.</exception>
        /// <exception cref="XspException">Thrown when other errors occur.</exception>
        void Verify(X509Certificate2 certificate);
    }
}
