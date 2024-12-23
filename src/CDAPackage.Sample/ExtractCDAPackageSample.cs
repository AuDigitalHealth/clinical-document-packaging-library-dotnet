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
using System.Security.Cryptography.X509Certificates;

namespace Nehta.VendorLibrary.CDAPackage.Sample
{
    public class ExtractCDAPackageSample
    {
        public void ExtractCDAPackage()
        {
            // Extract the contents of a CDA package file
            // Signature is verified on this call, with an exception thrown if the validation fails
            var newPackage = CDAPackageUtility.Extract(
                "CdaPackageFilePath.zip",
                VerifyCertificate
                );

            // Get CDA document content
            byte[] cdaRoodDocumentContent = newPackage.CDADocumentRoot.FileContent;

            // Get attachments if they exist
            if (newPackage.CDADocumentAttachments.Count == 2)
            {
                byte[] attachment1 = newPackage.CDADocumentAttachments[0].FileContent;
                string fileName1 = newPackage.CDADocumentAttachments[0].FileName;

                byte[] attachment2 = newPackage.CDADocumentAttachments[1].FileContent;
                string fileName2 = newPackage.CDADocumentAttachments[1].FileName;
            }
        }

        public void ExtractCDAPackageWithCheck()
        {
            CDAPackage newPackage;
            string message = "";

            // Extract the contents of a CDA package file and validate ths signature
            try
            {
                // Signature is verified on this call, with an exception thrown if the validation fails
                newPackage = CDAPackageUtility.Extract("CdaPackageFilePath.zip", VerifyCertificate);
            }
            catch (Exception ex)
            {
                // If exception thrown due to failed signature validation, capture the reason.
                // We recommend displaying the error to the user and continue to render the document.
                // This way the user can make a judgement call on whether or not to trust the
                // information contained within the document. 
                message = ex.Message;
                // No Signature validation checked to allow user to view package still
                newPackage = CDAPackageUtility.ExtractAndIgnoreSignatureVerification("CdaPackageFilePath.zip");
            }

            // Get CDA document content
            byte[] cdaRoodDocumentContent = newPackage.CDADocumentRoot.FileContent;

            // Get attachments if they exist
            if (newPackage.CDADocumentAttachments.Count == 2)
            {
                byte[] attachment1 = newPackage.CDADocumentAttachments[0].FileContent;
                string fileName1 = newPackage.CDADocumentAttachments[0].FileName;

                byte[] attachment2 = newPackage.CDADocumentAttachments[1].FileContent;
                string fileName2 = newPackage.CDADocumentAttachments[1].FileName;
            }
        }

        public void VerifyCertificate(X509Certificate2 certificate)
        {
            // This is an sample certificate check, which does an online revocation check.
            // In the future, there may be CDA packages which are signed with certificates 
            // which are valid at signing time, but have since been revoked or expired.
            // In this case, the certificate check should be relaxed. One such way is to
            // change the revocation mode to "no check". Eg:
            // chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            
            // Setup the chain
            var chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;

            // Perform the validation   
            chain.Build(certificate);

            // Check the results
            if (chain.ChainStatus.Length == 0)
            {
                // No errors found
            }
            else
            {
                // Errors found 
            }
        }
    }
}
