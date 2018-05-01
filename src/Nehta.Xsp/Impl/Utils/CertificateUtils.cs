/*
 * Copyright 2010 NEHTA
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


namespace Nehta.Xsp.Impl.Utils
{
  /// <summary>
  /// Utility class that contains certificate related methods.
  /// </summary>
  public static class CertificateUtils
  {
    /// <summary>
    /// Oid for the subject key identifier extension.
    /// </summary>
    public const string SubjectKeyIdentifierOid = "2.5.29.14";


    /// <summary>
    /// Returns the subject key identifier of a certificate.
    /// </summary>
    /// <param name="certificate">Certificate that contains the subject key identifier.
    /// Cannot be null.</param>
    /// <returns>Subject key identifier or null when the subject key identifier 
    /// does not exist.</returns>   
    public static X509SubjectKeyIdentifierExtension GetSubjectKeyIdentifier(
      X509Certificate2 certificate)
    {
      return (X509SubjectKeyIdentifierExtension)
        certificate.Extensions[SubjectKeyIdentifierOid];
    }
    
    /// <summary>
    /// Throws an 'ArgumentException' is an certificate collection is null
    /// or empty.
    /// </summary>
    /// <param name="certs">Certificate collection to test. Cannot be null.
    /// </param>
    /// <param name="argumentName">Name of the argument. Cannot be null.
    /// </param>
    /// <exception cref="ArgumentException">Thrown when the collection is
    /// null or empty.</exception>
    public static void CheckNotNullOrEmpty(X509Certificate2Collection certs, 
      string argumentName)
    {
      if (certs == null || certs.Count == 0)
      {
        throw new ArgumentException("'" + argumentName + 
          "' cannot be null or empty.");
      }
    }
    
    /// <summary>
    /// Checks if two certificates are equal.
    /// </summary>
    /// <param name="cert1">Certificate 1.</param>
    /// <param name="cert2">Certificate 2.</param>
    /// <returns>True if equal otherwise false.</returns>
    public static bool AreEqual(X509Certificate cert1, X509Certificate cert2)
    {
      if (cert1 == cert2)
      {
        return true;
      }
          
      if (cert1.GetRawCertData().Length != cert2.GetRawCertData().Length)
      {
        return false;
      }
      
      for (int i = 0; i < cert1.GetRawCertData().Length; i ++)
      {
        if (cert1.GetRawCertData()[i] != cert2.GetRawCertData()[i])
        {
          return false;
        }
      }
      
      return true;
    }
  }
}
