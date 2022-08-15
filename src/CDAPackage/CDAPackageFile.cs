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
using System.Xml;
using Nehta.VendorLibrary.Common;

namespace Nehta.VendorLibrary.CDAPackage
{
    /// <summary>
    /// This class may be used to create and manipulate a CDA package file.
    /// </summary>
    public class CDAPackageFile
    {
        /// <summary>
        /// Package file type
        /// </summary>
        public enum FileType
        {
            /// <summary>
            /// 
            /// </summary>
            RootDocument,
            /// <summary>
            /// 
            /// </summary>
            Signature,
            /// <summary>
            /// 
            /// </summary>
            Attachment
        }

        #region Public Properties

        /// <summary>
        /// Read only property that indicates the type of the CDAPackageFile.
        /// </summary>
        public FileType CDAPackageFileType { get; internal set; }

        /// <summary>
        /// The actual content of the file.
        /// </summary>
        public byte[] FileContent { get; internal set; }

        /// <summary>
        /// URI of file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Determines whether the specified Object is equal to the current Object.
        /// </summary>
        /// <param name="obj">The Object to compare with the current Object.</param>
        /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var compare = obj as CDAPackageFile;
            if (compare == null) return false;

            if (CDAPackageFileType != compare.CDAPackageFileType) return false;

            if (FileContent.Length != compare.FileContent.Length) return false;

            if (FileName.ToUpper() != compare.FileName.ToUpper()) return false;

            return true;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a CDA package file.
        /// </summary>
        internal CDAPackageFile()
        {
        }

        #endregion

    }

}
