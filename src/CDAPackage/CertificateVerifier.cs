/*
 * Copyright 2011 NEHTA
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
using System.Linq;
using System.Text;
using Nehta.Xsp;
using System.Security.Cryptography.X509Certificates;

namespace Nehta.VendorLibrary.CDAPackage
{
    /// <summary>
    /// Implementation of ICertificateVerifier. 
    /// </summary>
    internal class CertificateVerifier : ICertificateVerifier
    {
        VerifyCertificateDelegate verifyCertificate = null;

        /// <summary>
        /// Set the delegate for the verification method
        /// </summary>
        /// <param name="verifyCertificate">The certificate verification delegate.</param>
        public CertificateVerifier(VerifyCertificateDelegate verifyCertificate) 
        {
            this.verifyCertificate = verifyCertificate;
        }

        /// <summary>
        /// Call the delegate during the verification process.
        /// </summary>
        /// <param name="certificate">The certificate to verify.</param>
        void ICertificateVerifier.Verify(X509Certificate2 certificate)
        {
            if (verifyCertificate != null)
                verifyCertificate(certificate);
        }
    }
}
