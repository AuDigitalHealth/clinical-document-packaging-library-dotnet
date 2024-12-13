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
using System.Xml;
using Nehta.Xsp;
using System.Security.Cryptography.X509Certificates;
using Nehta.VendorLibrary.Common;
using Nehta.VendorLibrary.CDAPackage.XmlDsig;
using System.Security.Cryptography.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO.Compression;

namespace Nehta.VendorLibrary.CDAPackage
{
    /// <summary>
    /// Delegate for verify an certificate.
    /// </summary>
    /// <param name="certificate">The certificate to verify.</param>
    public delegate void VerifyCertificateDelegate(X509Certificate2 certificate);

    /// <summary>
    /// Utility class for CDAPackage related functions.
    /// </summary>
    public static class CDAPackageUtility
    {
        #region Public Methods
        
        /// <summary>
        /// Creates a signed CDA package zip file.
        /// </summary>
        /// <param name="package">The CDAPackageBase instance used to generate the zip file content.</param>
        /// <param name="signingCert">The certificate used to sign the CDA root document.</param>
        /// <returns>A byte array of the zip file content.</returns>
        public static byte[] Create(CDAPackage package, X509Certificate2 signingCert)
        {
            // Validation on package
            Validation.ValidateArgumentRequired("package", package);

            // Validate CDAPackage
            CDAPackageValidation.ValidateCDAPackage(package, signingCert != null);

            var ms = new MemoryStream();

            // Generate signature if package operation is ADD or REPLACE
            byte[] signatureContent = null;
            if (signingCert != null)
            {
                signatureContent = CreateSignature(package, signingCert);

                package.CDASignature = new CDAPackageFile();
                package.CDASignature.CDAPackageFileType = CDAPackageFile.FileType.Signature;
                package.CDASignature.FileContent = signatureContent;
                package.CDASignature.FileName = "CDA_SIGN.XML";
            }

            // Create Package
            var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                // Add folder entries
                archive.CreateEntry("IHE_XDM/");
                archive.CreateEntry("IHE_XDM/SUBSET01/");

                // CDA Doc
                var cdaDoc = archive.CreateEntry("IHE_XDM/SUBSET01/" + package.CDADocumentRoot.FileName);
                using (var stream = cdaDoc.Open())
                {
                    stream.Write(package.CDADocumentRoot.FileContent, 0, package.CDADocumentRoot.FileContent.Length);
                }

                // Add signature if present
                if (signatureContent != null)
                {
                    var cdaSign = archive.CreateEntry("IHE_XDM/SUBSET01/" + package.CDASignature.FileName);
                    using (var stream = cdaSign.Open())
                    {
                        stream.Write(package.CDASignature.FileContent, 0, package.CDASignature.FileContent.Length);
                    }
                }

                // CDA Attachments
                if (package.CDADocumentAttachments != null)
                {
                    foreach (var file in package.CDADocumentAttachments)
                    {
                        var cdaAttachment = archive.CreateEntry("IHE_XDM/SUBSET01/" + file.FileName);
                        using (var stream = cdaAttachment.Open())
                        {
                            stream.Write(file.FileContent, 0, file.FileContent.Length);
                        }
                    }
                }
            }

            return zipStream.ToArray();
        }

        /// <summary>
        /// Creates an unsigned CDA package zip file. Can be used for CdaPackageBase; and for the ADD, REPLACE or REMOVE operations.
        /// </summary>
        /// <param name="package">The CDAPackageBase instance used to generate the zip file content.</param>
        /// <returns>A byte array of the zip file content.</returns>
        public static byte[] Create(CDAPackage package)
        {
            return Create(package, null);
        }

        /// <summary>
        /// Creates a signed CDA package zip file. Can be used for CdaPackageBase; and for the ADD or REPLACE operations.
        /// </summary>
        /// <param name="package">The CDAPackageBase instance used to generate the zip file content.</param>
        /// <param name="outputFilePath">The path to output the zip file.</param>
        /// <param name="signingCert">The certificate used to sign the CDA root document.</param>
        public static void CreateZip(CDAPackage package, string outputFilePath, X509Certificate2 signingCert)
        {
            // Validation on package, outputFilePath
            Validation.ValidateArgumentRequired("package", package);
            Validation.ValidateArgumentRequired("outputFilePath", outputFilePath);

            var zipFileContent = Create(package, signingCert);

            File.WriteAllBytes(outputFilePath, zipFileContent);
        }

        /// <summary>
        /// Creates an unsigned CDA package zip file. Can be used for CdaPackageBase; and for the ADD, REPLACE or REMOVE operations.
        /// </summary>
        /// <param name="package">The CDAPackageBase instance used to generate the zip file content.</param>
        /// <param name="outputFilePath">The path to output the zip file.</param>
        public static void CreateZip(CDAPackage package, string outputFilePath)
        {
            CreateZip(package, outputFilePath, null);
        }
        
        /// <summary>
        /// Extracts a CDA package zip file and verifies the signature. An exception is thrown if verification fails.
        /// </summary>
        /// <param name="package">A byte array of a CDA package zip file.</param>
        /// <param name="verifyCertificate">An optional delegate to verify the signature certificate (NULL accepted).</param>
        /// <returns>A CDAPackage instance constructed from the CDA package zip file.</returns>
        public static CDAPackage Extract(byte[] package, VerifyCertificateDelegate verifyCertificate)
        {
            // Validation on package
            Validation.ValidateArgumentRequired("package", package);

            var newPackage = ExtractAndIgnoreSignatureVerification(package);

            if (newPackage.CDASignature != null)
                VerifySignature(newPackage, verifyCertificate);

            CDAPackageValidation.ValidateCDAPackage(newPackage, newPackage.SigningTime.HasValue);

            return newPackage;
        }

        /// <summary>
        /// Extracts a CDA package zip file and verifies the signature. An exception is thrown if verification fails.
        /// </summary>
        /// <param name="packageFilePath">The path to the CDA package zip file to extract.</param>
        /// <param name="verifyCertificate">An optional delegate to verify the signature certificate (NULL accepted).</param>
        /// <returns>A CDAPackage instance constructed from the CDA package zip file.</returns>
        public static CDAPackage Extract(string packageFilePath, VerifyCertificateDelegate verifyCertificate)
        {
            byte[] zipContent = File.ReadAllBytes(packageFilePath);

            CDAPackage newPackage = Extract(zipContent, verifyCertificate);

            return newPackage;
        }
        
        /// <summary>
        /// Extracts a CDA package zip file without verifying the signature.
        /// </summary>
        /// <param name="package">A byte array of a CDA package zip file.</param>
        /// <returns>A CDAPackage instance constructed from the CDA package zip file.</returns>
        public static CDAPackage ExtractAndIgnoreSignatureVerification(byte[] package)
        {
            CDAPackage extractedPackage = null;

            // Validation on package
            Validation.ValidateArgumentRequired("package", package);

            // Get zip entries in zip package
            Dictionary<string, byte[]> entries = GetZipEntriesFromZipStream(package);

            // Check to ensure that there is only one submission set folder
            string submissionPath = null;
            foreach (var name in entries.Keys)
            {
                var m = Regex.Match(name, @"^([^/\\]+[/\\][^/\\]+[/\\])[^/\\]+$", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    if (submissionPath == null) submissionPath = m.Groups[1].Value;
                    else if (submissionPath != m.Groups[1].Value)
                        throw new ArgumentException("More than one submission set folder found.");
                }
            }

            // Get root document
            var rootDoc = entries.FirstOrDefault(a => Regex.IsMatch(a.Key, @"^[^/\\]+[/\\][^/\\]+[/\\]CDA_ROOT.XML$", RegexOptions.IgnoreCase));
            if (string.IsNullOrEmpty(rootDoc.Key))
                throw new ArgumentException("CDA_ROOT.XML not found.");
                      
            // Get submission set folder
            string submissionSetPath = Regex.Match(rootDoc.Key, @"^([^/\\]+[/\\][^/\\]+[/\\])CDA_ROOT.XML$", RegexOptions.IgnoreCase).Groups[1].Value;

            var newPackage = new CDAPackage();

            newPackage.CDADocumentRoot = new CDAPackageFile();
            newPackage.CDADocumentRoot.CDAPackageFileType = CDAPackageFile.FileType.RootDocument;
            newPackage.CDADocumentRoot.FileName = rootDoc.Key.Replace(submissionPath, "");
            newPackage.CDADocumentRoot.FileContent = rootDoc.Value;

            foreach (var entry in entries)
            {
                if (entry.Key == rootDoc.Key)
                    continue;
                else if (entry.Key.Equals(submissionPath + "CDA_SIGN.XML", StringComparison.InvariantCultureIgnoreCase))
                {
                    var signature = new CDAPackageFile();
                    signature.CDAPackageFileType = CDAPackageFile.FileType.Signature;
                    signature.FileName = entry.Key.Replace(submissionPath, "");
                    signature.FileContent = entry.Value;

                    newPackage.CDASignature = signature;
                }
                else if (entry.Key.StartsWith(submissionPath, true, CultureInfo.InvariantCulture))
                {
                    var attachment = new CDAPackageFile();
                    attachment.CDAPackageFileType = CDAPackageFile.FileType.Attachment;
                    attachment.FileName = entry.Key.Replace(submissionPath, "");
                    attachment.FileContent = entry.Value;

                    if (newPackage.CDADocumentAttachments == null)
                        newPackage.CDADocumentAttachments = new List<CDAPackageFile>();

                    newPackage.CDADocumentAttachments.Add(attachment);
                }
            }

            extractedPackage = newPackage;

            return extractedPackage;
        }
         
        /// <summary>
        /// Verify CDA package attachments integrity checks.
        /// </summary>
        /// <param name="package">The CDA package to verify.</param>
        public static void VerifyAttachments(CDAPackage package)
        {
            List<KeyValuePair<string, string>> hashFails = new List<KeyValuePair<string, string>>();

            // Verify document attachments
            XmlDocument rootDocument = new XmlDocument();
            try
            {
                rootDocument.Load(new MemoryStream(package.CDADocumentRoot.FileContent));
            }
            catch (Exception)
            {
                throw new ValidationException("package", null, "CDA_ROOT.XML cannot be extracted as an XML document.");
            }

            XmlNamespaceManager nm = new XmlNamespaceManager(rootDocument.NameTable);
            nm.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            nm.AddNamespace("nm", "urn:hl7-org:v3");

            var nodeList = rootDocument.SelectNodes("//nm:value[@integrityCheck!='' and descendant::nm:reference[@value!='']]", nm);
            for (int x = 0; x < nodeList.Count; x++)
            {
                var node = nodeList[x];
                string attachmentHash = node.Attributes["integrityCheck"].Value;
                string attachmentUri = node.SelectSingleNode("nm:reference", nm).Attributes["value"].Value.ToLower();
                string algorithm = "sha-1";
                if (node.Attributes["integrityCheckAlgorithm"] != null && !string.IsNullOrEmpty(node.Attributes["integrityCheckAlgorithm"].Value))
                    algorithm = node.Attributes["integrityCheckAlgorithm"].Value.ToLower();

                foreach (var attachment in package.CDADocumentAttachments.Where(a => a.FileName.ToLower() == attachmentUri))
                {
                    string calculatedHash = null;

                    if (algorithm == "sha-1")
                        calculatedHash = Convert.ToBase64String(CalculateSHA1(attachment.FileContent));
                    else if (algorithm == "sha-256")
                        calculatedHash = Convert.ToBase64String(CalculateSHA256(attachment.FileContent));

                    if (calculatedHash != attachmentHash) 
                        hashFails.Add(new KeyValuePair<string, string>(attachmentUri, attachmentHash));
                }
            }

            if (hashFails.Count > 0)
            {
                string exceptionList = "Error verifying document reference: ";
                foreach (var pair in hashFails)
                {
                    exceptionList += string.Format("\n- {0} ({1})", pair.Key, pair.Value);
                }

                throw new SignatureVerificationException(exceptionList);
            }
        }

        /// <summary>
        /// Verify the signature of a CDA package.
        /// </summary>
        /// <param name="package">The CDA package to verify.</param>
        /// <param name="verifyCertificate">A delegate to verify the signature certificate.</param>
        public static void VerifySignature(CDAPackage package, VerifyCertificateDelegate verifyCertificate)
        {
            // Return if package doesn't contain a signature.
            if (package.CDASignature == null)
                return;

            byte[] signatureDocumentContent = package.CDASignature.FileContent;
            byte[] rootDocumentContent = package.CDADocumentRoot.FileContent;

            byte[] hash = CalculateSHA1(rootDocumentContent);

            var signatureDocument = new XmlDocument();
            signatureDocument.PreserveWhitespace = true;
            signatureDocument.Load(new MemoryStream(signatureDocumentContent));

            // Get eSignature
            eSignatureType eSignature = null;

            try
            {
                var eSignatureElement = signatureDocument.GetElementsByTagName("eSignature", "*")[0] as XmlElement;
                eSignature = eSignatureElement.Deserialize<eSignatureType>();
            }
            catch (Exception)
            {
                throw new SignatureVerificationException("Error extracting eSignature");
            }

            if (eSignature == null)
                throw new SignatureVerificationException("Error extracting eSignature");

            var manifest = eSignature.Manifest;

            var approver = eSignature.approver;

            // Get signing time

            package.SigningTime = eSignature.signingTime;

            // Get approver

            package.Approver = new Approver();
            if (eSignature.approver != null)
            {
                if (eSignature.approver.personId != null)
                    package.Approver.PersonId = new Uri(eSignature.approver.personId, UriKind.RelativeOrAbsolute);
                var personName = eSignature.approver.personName;
                if (personName != null)
                {
                    package.Approver.PersonFamilyName = personName.familyName;
                    if (personName.givenName != null)
                        package.Approver.PersonGivenNames = personName.givenName.ToList();
                    if (personName.nameSuffix != null)
                        package.Approver.PersonNameSuffixes = personName.nameSuffix.ToList();
                    if (personName.nameTitle != null)
                        package.Approver.PersonTitles = personName.nameTitle.ToList();
                }
            }

            // Check signature digest

            var manifestElement = signatureDocument.GetElementsByTagName("Manifest", "*")[0] as XmlElement;

            if (manifest.Reference[0].URI != "CDA_ROOT.XML")
                throw new SignatureVerificationException("Error verifying document - Manifest reference must have a URI of 'CDA_ROOT.XML'");

            if (manifest.Reference[0].DigestMethod.Algorithm != SignedXml.XmlDsigSHA1Url)
                throw new SignatureVerificationException("Error verifying document - Manifest digest method must have an algorithm of '" + SignedXml.XmlDsigSHA1Url + "'");

            if (Convert.ToBase64String(manifest.Reference[0].DigestValue) != Convert.ToBase64String(hash))
                throw new SignatureVerificationException("Error verifying document - Manifest digest value mismatch");

            // Verify certificate

            ICertificateVerifier verifier = new CertificateVerifier(verifyCertificate);

            ISignedContainerProfileService signedContainerService = XspFactory.Instance.GetSignedContainerProfileService(XspVersion.V_2010);
            signedContainerService.Check(signatureDocument, verifier);

            // Verify attachments integrity checks
            if (package.CDADocumentAttachments != null && package.CDADocumentAttachments.Count > 0)
                VerifyAttachments(package);

            return;
        }

        #endregion 

        #region Private and internal methods

        /// <summary>
        /// Obtain all the zip file entries and their content.
        /// </summary>
        /// <param name="zipFile">The zip file.</param>
        /// <returns>Zip file entries and their content.</returns>
        internal static Dictionary<string, byte[]> GetZipEntriesFromZipStream(byte[] zipFile)
        {
            var contentByFileName = new Dictionary<string, byte[]>();
            var inputStream = new MemoryStream(zipFile);

            if (zipFile.Length > 0)
            {
                using (ZipArchive zipArchive = new ZipArchive(inputStream, ZipArchiveMode.Read))
                {
                    foreach (var entry in zipArchive.Entries)
                    {
                        // Ony process files.
                        if (entry.Length > 0)
                        {
                            var output = new MemoryStream();
                            entry.Open().CopyTo(output);
                            contentByFileName.Add(entry.FullName, output.ToArray());
                        }
                    }
                }
            }

            return contentByFileName;
        }

        /// <summary>
        /// Signs the CDA Package and creates the signature document.
        /// </summary>
        /// <param name="package">A CDAPackageBase instance containing the root document to sign.</param>
        /// <param name="signingCert">The certificate used to sign the root document.</param>
        /// <returns>Signature of the root document.</returns>
        private static byte[] CreateSignature(CDAPackage package, X509Certificate2 signingCert)
        {
            package.SigningTime = DateTime.Now.ToUniversalTime();

            byte[] rootDocumentContent = package.CDADocumentRoot.FileContent;

            byte[] hash = CalculateSHA1(rootDocumentContent);

            var manifest = new ManifestType();
            manifest.Reference = new ReferenceType[]
            {
                new ReferenceType() {
                    URI = package.CDADocumentRoot.FileName,
                    DigestMethod = new DigestMethodType()
                    {
                        Algorithm = SignedXml.XmlDsigSHA1Url
                    },
                    DigestValue = hash
                }
            };

            var approver = new ApproverType();
            approver.personId = package.Approver.PersonId.ToString();
            approver.personName = new PersonNameType();
            approver.personName.familyName = package.Approver.PersonFamilyName;
            if (package.Approver.PersonTitles != null)
                approver.personName.nameTitle = package.Approver.PersonTitles.ToArray();
            if (package.Approver.PersonGivenNames != null)
                approver.personName.givenName = package.Approver.PersonGivenNames.ToArray();
            if (package.Approver.PersonNameSuffixes != null)
                approver.personName.nameSuffix = package.Approver.PersonNameSuffixes.ToArray();

            var eSignature = new eSignatureType();
            eSignature.Manifest = manifest;
            eSignature.approver = approver;
            eSignature.signingTime = package.SigningTime.Value;

            XmlDocument eSignatureXml = eSignature.SerializeToXml();

            ISignedContainerProfileService signedContainerService = XspFactory.Instance.GetSignedContainerProfileService(XspVersion.V_2010);

            XmlDocument signedDoc = signedContainerService.Create(eSignatureXml, signingCert);

            var ms = new MemoryStream();
            signedDoc.Save(ms);

            return ms.ToArray();
        }

        /// <summary>
        /// Generates a hash value for a byte array using SHA1.
        /// </summary>
        /// <param name="content">The byte array to generate the hash from.</param>
        /// <returns>Generated hash value.</returns>
        private static byte[] CalculateSHA1(byte[] content)
        {
            var cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            return cryptoTransformSHA1.ComputeHash(content);
        }

        /// <summary>
        /// Generates a hash value for a byte array using SHA256.
        /// </summary>
        /// <param name="content">The byte array to generate the hash from.</param>
        /// <returns>Generated hash value.</returns>
        private static byte[] CalculateSHA256(byte[] content)
        {
            var cryptoTransformSHA256 = new SHA256CryptoServiceProvider();
            return cryptoTransformSHA256.ComputeHash(content);
        }

        #endregion 
    }
}
