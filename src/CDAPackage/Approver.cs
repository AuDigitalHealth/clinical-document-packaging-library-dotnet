using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nehta.VendorLibrary.CDAPackage
{
    /// <summary>
    /// Represents a CDA signature approver.
    /// </summary>
    public class Approver
    {
        public Uri PersonId { get; set; }
        public List<string> PersonTitles { get; set; }
        public List<string> PersonGivenNames { get; set; }
        public string PersonFamilyName { get; set; }
        public List<string> PersonNameSuffixes { get; set; }

        public Approver()
        {
        }

        public Approver(Uri personId, string personFamilyName)
        {
            this.PersonId = personId;
            this.PersonFamilyName = personFamilyName;
        }

        /// <summary>
        /// Determines whether the specified Object is equal to the current Object.
        /// </summary>
        /// <param name="obj">The Object to compare with the current Object.</param>
        /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var compare = obj as Approver;
            if (compare == null) return false;

            if (Helper.HasNullDifference(PersonId, compare.PersonId)) return false;
            if (PersonId != null && (PersonId.ToString() != compare.PersonId.ToString())) return false;

            if (PersonFamilyName != compare.PersonFamilyName) return false;

            if (!StringListCompare(PersonGivenNames, compare.PersonGivenNames)) return false;
            if (!StringListCompare(PersonNameSuffixes, compare.PersonNameSuffixes)) return false;
            if (!StringListCompare(PersonTitles, compare.PersonTitles)) return false;
            
            return true;
        }

        private bool StringListCompare(List<string> array1, List<string> array2)
        {
            if (Helper.HasNullDifference(array1, array2)) return false;
            if (array1 != null)
            {
                array1.Sort((a, b) => a.CompareTo(b));
                array2.Sort((a, b) => a.CompareTo(b));

                for (int x = 0; x < array1.Count; x++)
                {
                    if (array1[x] != array2[x]) return false;
                }
            }
            return true;
        }
    }
}
