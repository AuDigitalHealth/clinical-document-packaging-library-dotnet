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
using System.Collections.Generic;
using Nehta.Xsp.Impl.V1;


namespace Nehta.Xsp
{
  /// <summary>
  /// Factory for creating implementations of the E-Health Xml Secured Payload
  /// Profile technical specification.
  /// </summary>
  public class XspFactory
  {
    /// <summary>
    /// Singleton instance.
    /// </summary>
    private static XspFactory instance = new XspFactory();

    /// <summary>
    /// Signed container profile implementations.
    /// </summary>
    private IDictionary<XspVersion, ISignedContainerProfileService> 
      signedContainerProfileServices;

    /// <summary>
    /// Xml Signature profile implementations.
    /// </summary>
    private IDictionary<XspVersion, IXmlSignatureProfileService> 
      signedProfileServices;

    /// <summary>
    /// Encrypted container profile implementations.
    /// </summary>
    private IDictionary<XspVersion, IEncryptedContainerProfileService> 
      encryptedContainerProfileServices;
    
    /// <summary>
    /// Xml encryption profile implementations.
    /// </summary>
    private IDictionary<XspVersion, IXmlEncryptionProfileService> 
      encryptedProfileServices;

    
    /// <summary>
    /// Constructor.
    /// </summary>
    private XspFactory()
    {
      IXmlSignatureProfileService signedProfileService = new XmlSignatureProfileService();

      this.signedProfileServices = new Dictionary<XspVersion, IXmlSignatureProfileService>();
      this.signedProfileServices.Add(XspVersion.V_1_2, signedProfileService);
      this.signedProfileServices.Add(XspVersion.V_2010, signedProfileService);

      this.signedContainerProfileServices = 
        new Dictionary<XspVersion, ISignedContainerProfileService>();

      this.signedContainerProfileServices.Add(XspVersion.V_1_2, 
        new SignedContainerProfileService(XspNamespaceConstants.NsSignedPayload_V_1_2,
        signedProfileService));

      this.signedContainerProfileServices.Add(XspVersion.V_2010,
        new SignedContainerProfileService(XspNamespaceConstants.NsSignedPayload_V_2010,
        signedProfileService));

      IXmlEncryptionProfileService encryptedProfileService = 
        new XmlEncryptionProfileService();

      this.encryptedProfileServices = 
        new Dictionary<XspVersion, IXmlEncryptionProfileService>();
      this.encryptedProfileServices.Add(XspVersion.V_1_2, encryptedProfileService);
      this.encryptedProfileServices.Add(XspVersion.V_2010, encryptedProfileService);

      this.encryptedContainerProfileServices = 
        new Dictionary<XspVersion, IEncryptedContainerProfileService>();

      this.encryptedContainerProfileServices.Add(XspVersion.V_1_2,
        new EncryptedContainerProfileService(XspNamespaceConstants.NsEncryptedPayload_V_1_2,
          encryptedProfileService));

      this.encryptedContainerProfileServices.Add(XspVersion.V_2010,
        new EncryptedContainerProfileService(XspNamespaceConstants.NsEncryptedPayload_V_2010,
          encryptedProfileService));
    }

    /// <summary>
    /// Returns an instance of the factory.
    /// </summary>
    /// <returns></returns>
    public static XspFactory Instance
    {
      get 
      {
        return instance;
      }
    }
       
    /// <summary>
    /// Gets a an instance of a signed profile service with the specified version.
    /// </summary>
    /// <param name="xspVersion">Xsp version.</param>
    /// <returns>Instance.</returns>
    public IXmlSignatureProfileService GetSignedProfileService(XspVersion xspVersion)
    {
      return this.signedProfileServices[xspVersion];
    }

    /// <summary>
    /// Gets a an instance of a signed container profile service with the specified version.
    /// </summary>
    /// <param name="xspVersion">Xsp version.</param>
    /// <returns>Instance.</returns>
    public ISignedContainerProfileService GetSignedContainerProfileService(
      XspVersion xspVersion)
    {
      return this.signedContainerProfileServices[xspVersion];
    }

    /// <summary>
    /// Gets a an instance of a encrypted profile service with the specified version.
    /// </summary>
    /// <param name="xspVersion">Xsp version.</param>
    /// <returns>Instance.</returns>
    public IXmlEncryptionProfileService GetEncryptedProfileService(XspVersion xspVersion)
    {
      return this.encryptedProfileServices[xspVersion];
    }

    /// <summary>
    /// Gets a an instance of a encrypted container profile service with the specified version.
    /// </summary>
    /// <param name="xspVersion">Xsp version.</param>
    /// <returns>Instance.</returns>
    public IEncryptedContainerProfileService GetEncryptedContainerProfileService(
      XspVersion xspVersion)
    {
      return this.encryptedContainerProfileServices[xspVersion];
    }

  }
}
