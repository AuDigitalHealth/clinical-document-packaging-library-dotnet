using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nehta.VendorLibrary.CDAPackage
{
    /// <summary>
    /// Exception related to signature verification
    /// </summary>
    public class SignatureVerificationException : ApplicationException
    {
        /// <summary>
        /// Creates a SignatureVerificationException
        /// </summary>
        /// <param name="message">Exception message</param>
        public SignatureVerificationException(string message)
            : base(message)
        {
        }
    }
}
