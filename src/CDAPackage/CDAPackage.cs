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
using Nehta.VendorLibrary.Common;

namespace Nehta.VendorLibrary.CDAPackage
{
    /// <summary>
    /// Represents a CDA package with metadata information.
    /// </summary>
    public class CDAPackage
    {
        #region Public Properties

        /// <summary>
        /// The CDA root document.
        /// </summary>
        public CDAPackageFile CDADocumentRoot { get; set; }

        /// <summary>
        /// A list of CDA Document Attachments.
        /// </summary>
        public List<CDAPackageFile> CDADocumentAttachments { get; set; }

        /// <summary>
        /// The CDA signature document.
        /// </summary>
        public CDAPackageFile CDASignature;

        /// <summary>
        /// The approver of the CDA document.
        /// </summary>
        public Approver Approver;

        /// <summary>
        /// The date and time the CDA package is signed.
        /// </summary>
        public DateTime? SigningTime { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a CDA package.
        /// </summary>
        public CDAPackage(Approver approver)
        {
            // Validation
            var vb = new ValidationBuilder();

            vb.ArgumentRequiredCheck("approver", approver);

            if (approver != null)
            {
                vb.ArgumentRequiredCheck("approver.PersonFamilyName", approver.PersonFamilyName);
                vb.ArgumentRequiredCheck("approver.PersonId", approver.PersonId);
            }

            if (vb.Messages.Count > 0)
                throw new ValidationException(vb.Messages);

            this.Approver = approver;
        }

        internal CDAPackage()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a root document in the CDA package.
        /// </summary>
        /// <param name="fileContent">The content of the file.</param>
        public void CreateRootDocument(byte[] fileContent)
        {
            // Validation
            var vb = new ValidationBuilder();

            vb.ArgumentRequiredCheck("fileContent", fileContent);

            if (vb.Messages.Count > 0)
                throw new ValidationException(vb.Messages);

            var file = new CDAPackageFile();

            // Indicate that this is the root document
            file.CDAPackageFileType = CDAPackageFile.FileType.RootDocument;

            // Set file content
            file.FileContent = fileContent;

            // Known fields
            file.FileName = "CDA_ROOT.XML";

            CDADocumentRoot = file;
        }

        /// <summary>
        /// Adds an attachment to the CDA Package.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileContent">The content of the file.</param>
        public void AddDocumentAttachment(
            string fileName,
            byte[] fileContent)
        {
            // Validation
            var vb = new ValidationBuilder();

            vb.ArgumentRequiredCheck("fileName", fileName);
            vb.ArgumentRequiredCheck("fileContent", fileContent);

            if (vb.Messages.Count > 0)
                throw new ValidationException(vb.Messages);

            var file = new CDAPackageFile();

            // Indicate that this is a document attachment
            file.CDAPackageFileType = CDAPackageFile.FileType.Attachment;

            // User specified fields
            file.FileName = fileName;
            file.FileContent = fileContent;

            if (CDADocumentAttachments == null)
                CDADocumentAttachments = new List<CDAPackageFile>();

            CDADocumentAttachments.Add(file);
        }

        /// <summary>
        /// Determines whether the specified Object is equal to the current Object.
        /// </summary>
        /// <param name="obj">The Object to compare with the current Object.</param>
        /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var compare = obj as CDAPackage;
            if (compare == null) return false;

            if (CDADocumentAttachments != null)
            {
                if (compare.CDADocumentAttachments == null) return false;
                if (CDADocumentAttachments.Count != compare.CDADocumentAttachments.Count) return false;

                CDADocumentAttachments.Sort((a, b) => a.FileName.ToString().CompareTo(b.FileName.ToString()));
                compare.CDADocumentAttachments.Sort((a, b) => a.FileName.ToString().CompareTo(b.FileName.ToString()));

                for (int x = 0; x < CDADocumentAttachments.Count; x++)
                {
                    if (!Helper.EqualsCompare(CDADocumentAttachments[x], compare.CDADocumentAttachments[x])) return false;
                }
            }
            else if (compare.CDADocumentAttachments != null)
                return false;

            if (!Helper.EqualsCompare(CDADocumentRoot, compare.CDADocumentRoot)) return false;
            if (!Helper.EqualsCompare(CDASignature, compare.CDASignature)) return false;

            if (SigningTime != null || compare.SigningTime != null)
            {
                if (!SigningTime.Equals(compare.SigningTime)) return false;
            }

            if (Helper.HasNullDifference(Approver, compare.Approver)) return false;
            if (Approver != null && !Approver.Equals(compare.Approver)) return false;

            return true;
        }

        #endregion
    }
}
