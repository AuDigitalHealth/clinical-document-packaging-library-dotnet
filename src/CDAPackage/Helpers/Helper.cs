using System;

namespace Nehta.VendorLibrary.CDAPackage
{
    internal class Helper
    {
        internal static bool EqualsCompare(object obj1, object obj2)
        {
            if (obj1 != null && obj1.Equals(obj2))
                return true;
            else if (obj1 == null && obj2 == null)
                return true;
            else
                return false;
        }

        internal static bool HasNullDifference(object obj1, object obj2)
        {
            if (obj1 == null && obj2 != null)
                return true;
            else if (obj1 != null && obj2 == null)
                return true;
            else
                return false;
        }

        internal static bool DateTimeCompare(DateTime? date1, DateTime? date2)
        {
            if (date1 != null && date2 != null && date1.Value.ToString("yyyyMMddHHmmss") == date2.Value.ToString("yyyyMMddHHmmss"))
                return true;
            else if (date1 == null && date2 == null)
                return true;
            else
                return false;
        }
    }
}
