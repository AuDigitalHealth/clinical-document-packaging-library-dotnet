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

using Nehta.VendorLibrary.Common;

namespace Nehta.VendorLibrary.CDAPackage
{
    /// <summary>
    /// Helper class for CDAPackage validation.
    /// </summary>
    internal class CDAPackageValidation
    {
        /// <summary>
        /// Validates a CDAPackage.
        /// </summary>
        /// <param name="package">The CDAPackage to validate.</param>
        /// <param name="validateApprover">Validate Approver.</param>
        internal static void ValidateCDAPackage(CDAPackage package, bool validateApprover)
        {
            ValidationBuilder vb = new ValidationBuilder("package");

            // Validate approver
            if (validateApprover)
            {
                vb.ArgumentRequiredCheck("Approver", package.Approver);

                if (package.Approver != null)
                {
                    vb.ArgumentRequiredCheck("Approver.PersonFamilyName", package.Approver.PersonFamilyName);
                    vb.ArgumentRequiredCheck("Approver.PersonId", package.Approver.PersonId);
                }
            }

            // Validate root
            if (vb.ArgumentRequiredCheck("CDADocumentRoot", package.CDADocumentRoot))
            {
                vb.ArgumentRequiredCheck("CDADocumentRoot.FileName", package.CDADocumentRoot.FileName);
                vb.ArgumentRequiredCheck("CDADocumentRoot.FileContent", package.CDADocumentRoot.FileContent);
            }

            // Validate attachments
            if (package.CDADocumentAttachments != null) {
                for (int x = 0; x < package.CDADocumentAttachments.Count; x++)
                {
                    vb.ArgumentRequiredCheck(string.Format("CDADocumentAttachments[{0}].FileName", x), package.CDADocumentAttachments[x].FileName);
                    vb.ArgumentRequiredCheck(string.Format("CDADocumentAttachments[{0}].FileContent", x), package.CDADocumentAttachments[x].FileContent);
                }
            }

            if (vb.Messages.Count > 0)
            {
                throw new ValidationException(vb.Messages);
            }
        }

        /*
        #region Private methods

        /// <summary>
        /// Validate a CDAPackageFile.
        /// </summary>
        private static void ValidateCDAPackageFile(CDAPackageOperation operation, string name, CDAPackageFile file, ValidationBuilder vb)
        {
            ValidateXON(name + ".AuthorInstitution", file.AuthorInstitution, false, vb);
            ValidateXCN(name + ".AuthorPerson", file.AuthorPerson, false, vb);
            vb.ArgumentRequiredCheck(name + ".ClassCode", file.ClassCode);
            vb.ArgumentRequiredCheck(name + ".ConfidentialityCode", file.ConfidentialityCode);
            vb.ArgumentRequiredCheck(name + ".CreationTime", file.CreationTime);
            vb.UuidCheck(name + ".EntryUUID", file.EntryUUID, true);
            vb.ArgumentRequiredCheck(name + ".FormatCode", file.FormatCode);
            vb.ArgumentRequiredCheck(name + ".HealthcareFacilityTypeCode", file.HealthcareFacilityTypeCode);
            vb.ArgumentRequiredCheck(name + ".LanguageCode", file.LanguageCode);
            bool hasMimeType = vb.ArgumentRequiredCheck(name + ".MimeType", file.MimeType);
            ValidateCX(name + ".PatientId", file.PatientId, true, vb);
            vb.ArgumentRequiredCheck(name + ".PracticeSettingCode", file.PracticeSettingCode);
            ValidateCX(name + ".SourcePatientId", file.SourcePatientId, true, vb);
            vb.ArgumentRequiredCheck(name + ".TypeCode", file.TypeCode);
            vb.UuidCheck(name + ".UniqueId", file.UniqueId, true);
            bool hasURI = vb.ArgumentRequiredCheck(name + ".URI", file.URI);

            if (file.CDAPackageFileType == CDAPackageFile.FileType.RootDocument)
            {
                if (hasMimeType)
                    vb.MatchCheck<string>(name + ".MimeType", file.MimeType.ToLower(), "application/xml");

                if (hasURI)
                    vb.MatchCheck<string>(name + ".URI", file.URI.ToLower(), "cda_root.xml");

                // ADD operation: Try loading XML document, and also check for stylesheet
                if (operation == CDAPackageOperation.Add)
                {
                    try
                    {
                        var rootDoc = new XmlDocument();
                        rootDoc.Load(new MemoryStream(file.FileContent));

                        if (rootDoc.InnerXml.Contains("<?xml-stylesheet"))
                            vb.AddValidationMessage(name + ".FileContent", null,
                                                    "Unpermitted stylesheet element found within XML");
                    }
                    catch (Exception ex)
                    {
                        vb.AddValidationMessage(name + ".FileContent", null, "Error parsing content as XML");
                    }
                }
            }
            else if (file.CDAPackageFileType == CDAPackageFile.FileType.Signature)
            {
                if (hasMimeType)
                    vb.MatchCheck<string>(name + ".MimeType", file.MimeType.ToLower(), "application/xml");

                if (hasURI)
                    vb.MatchCheck<string>(name + ".URI", file.URI.ToLower(), "cda_sign.xml");
            }

            // If not REMOVE, then remove reason cannot be specified
            if (operation != CDAPackageOperation.Remove && file.RemoveReason != null)
                vb.AddValidationMessage(name + ".RemoveReason", null, "Argument can only be specified for the REMOVE operation");
        }
        
        /// <summary>
        /// Validates an XON instance.
        /// </summary>
        private static bool ValidateXON(string name, XON xon, bool required, ValidationBuilder vb)
        {
            var results = new List<bool>();

            if (required)
                results.Add(vb.ArgumentRequiredCheck(name, xon));
            
            if (xon != null)
            {
                results.Add(vb.OidQualifiedIdCheck(name + ".FullyQualifiedOrganisationIdentifier", xon.FullyQualifiedOrganisationIdentifier, true));
                results.Add(vb.ArgumentRequiredCheck(name + ".OrganisationName", xon.OrganisationName));
            }

            return results.Count(a => a == false) == 0;
        }

        /// <summary>
        /// Validates an XCN instance.
        /// </summary>
        private static bool ValidateXCN(string name, XCN xcn, bool required, ValidationBuilder vb)
        {
            var results = new List<bool>();

            if (required)
                results.Add(vb.ArgumentRequiredCheck(name, xcn));

            if (xcn != null)
            {
                if (string.IsNullOrEmpty(xcn.Identifier) && string.IsNullOrEmpty(xcn.LastName))
                {
                    results.Add(false);
                    vb.AddValidationMessage(vb.Path + name + ".Identifier, " + vb.Path + name + ".LastName", null, "Either of these must be specified");
                }

                if (!string.IsNullOrEmpty(xcn.Identifier))
                    results.Add(vb.OidCheck(name + ".OidOfAssigningAuthority", xcn.OidOfAssigningAuthority, false));
            }

            return results.Count(a => a == false) == 0;
        }

        /// <summary>
        /// Validates a CX instance.
        /// </summary>
        private static bool ValidateCX(string name, CX cx, bool required, ValidationBuilder vb)
        {
            var results = new List<bool>();

            if (required)
                results.Add(vb.ArgumentRequiredCheck(name, cx));

            if (cx != null)
            {
                results.Add(vb.ArgumentRequiredCheck(name + ".Identifier", cx.Identifier));
                results.Add(vb.OidCheck(name + ".OidOfAssigningAuthority", cx.OidOfAssigningAuthority, true));
            }

            return results.Count(a => a == false) == 0;
        }

        #endregion
        */
    }
}
