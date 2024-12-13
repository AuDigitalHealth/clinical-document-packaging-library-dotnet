using System;

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
