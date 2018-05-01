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
namespace Nehta.Xsp.Impl.V1
{
  /// <summary>
  /// Xsp namespace constants.
  /// </summary>
  public static class XspNamespaceConstants
  {  
    /// <summary>
    /// Encrypted payload container namespace for version 1.2
    /// </summary>
    public const string NsEncryptedPayload_V_1_2 = 
      "http://ns.nehta.gov.au/CoreConnectivity/Xsd/EncryptedPayload/1.2";

    /// <summary>
    /// Signed payload container namespace for version 1.2
    /// </summary>
    public const string NsSignedPayload_V_1_2 = 
      "http://ns.nehta.gov.au/CoreConnectivity/Xsd/SignedPayload/1.2";

    /// <summary>
    /// Encrypted payload container namespace for version 2010
    /// </summary>
    public const string NsEncryptedPayload_V_2010 = 
      "http://ns.electronichealth.net.au/xsp/xsd/EncryptedPayload/2010";

    /// <summary>
    /// Signed payload container namespace for version 2010
    /// </summary>
    public const string NsSignedPayload_V_2010 = 
      "http://ns.electronichealth.net.au/xsp/xsd/SignedPayload/2010";
  }
}
