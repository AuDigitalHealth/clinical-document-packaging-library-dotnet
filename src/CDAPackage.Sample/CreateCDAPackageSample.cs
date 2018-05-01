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
using System.Security.Cryptography.X509Certificates;
using Nehta.VendorLibrary.Common;
using System.Xml;
using System.IO;

namespace Nehta.VendorLibrary.CDAPackage.Sample
{
    public class CreateCDAPackageSample
    {
        public void CreateCDAPackage()
        {
            // ------------------------------------------------------------------------------
            // Set up signing certificate and identifiers
            // ------------------------------------------------------------------------------

            // Load certificate used to sign the CDA document
            X509Certificate2 signingCert = X509CertificateUtil.GetCertificate(
                "Signing Certificate Find Value",
                X509FindType.FindBySubjectName,
                StoreName.My,
                StoreLocation.CurrentUser,
                true);

            // ------------------------------------------------------------------------------
            // Create CDAPackage
            // ------------------------------------------------------------------------------

            // Create an approver
            var approver = new Approver()
                               {
                                   PersonId = new Uri("http://ns.electronichealth.net.au/id/hi/hpii/1.0/8003610000000000"),
                                   PersonFamilyName = "Jacobs",
                                   PersonGivenNames = new List<string> { "Adam", "Barry" },
                                   PersonNameSuffixes = new List<string> { "MD", "JR" },
                                   PersonTitles = new List<string> { "MR", "DR" }
                               };

            // Create a CDAPackage instance
            var package = new CDAPackage(approver);

            // Create the CDA root document for the CDA package
            package.CreateRootDocument(File.ReadAllBytes("CdaDocumentXmlFile.xml"));

            // Add an image attachment
            package.AddDocumentAttachment(
                "ImageAttachment1.jpg",
                File.ReadAllBytes("ImageAttachment1.jpg")
                );

            // Add another image attachment
            package.AddDocumentAttachment(
                "ImageAttachment2.png",
                File.ReadAllBytes("ImageAttachment2.png")
                );

            // Create the CDA package zip
            CDAPackageUtility.CreateZip(package, "CdaPackageOutputFilePath.zip", signingCert);
        }

    }
}
