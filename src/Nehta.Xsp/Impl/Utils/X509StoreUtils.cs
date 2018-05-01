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
    /// Certificate store utility class.
    /// </summary>
  public static class X509StoreUtils
  {
    /// <summary>
    /// Gets a certificate from the Windows certificate repository.
    /// </summary>
    /// <param name="findValue">Find value</param>
    /// <param name="findType">Find type</param>
    /// <returns>Matching certificate</returns>
    public static X509Certificate2 GetCertificate(String findValue,
      X509FindType findType)
    {
      return GetCertificate(findValue, findType, StoreName.My,
        StoreLocation.CurrentUser, true);
    }

    /// <summary>
    /// Gets a certificate from the Windows certificate repository.
    /// </summary>
    /// <param name="findValue">Find value</param>
    /// <param name="findType">Find type</param>
    /// <param name="storeName">Store name</param>
    /// <param name="storeLocation">Store location</param>
    /// <param name="valid">Valid certificate flag</param>
    /// <returns>Matching certificate</returns>
    /// <exception cref="ArgumentException">Thrown when no certificate matches or more than one match was found.</exception>
    public static X509Certificate2 GetCertificate(String findValue,
      X509FindType findType, StoreName storeName,
      StoreLocation storeLocation, bool valid)
    {
      X509Store certStore = new X509Store(storeName, storeLocation);
      certStore.Open(OpenFlags.ReadOnly);

      X509Certificate2Collection foundCerts =
        certStore.Certificates.Find(findType, findValue, valid);
      certStore.Close();

      // Check if any certificates were found with the criteria
      if (foundCerts.Count == 0)
      {
        throw new ArgumentException(
          "Certificate was not found with criteria '" + findValue + "'");
      }

      // Check if more than one certificate was found with the criteria
      if (foundCerts.Count > 1)
      {
        throw new ArgumentException(
          "More than one certificate found with criteria '" + findValue + "'");
      }

      return foundCerts[0];
    }

  }
}
