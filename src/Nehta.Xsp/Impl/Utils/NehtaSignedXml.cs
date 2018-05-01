using System.Security.Cryptography.Xml;
using System.Xml;

namespace Nehta.Xsp.Impl.Utils
{
    class NehtaSignedXml : SignedXml
    {
        private const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";
        private const string spNamespace = "http://ns.electronichealth.net.au/xsp/xsd/SignedPayload/2010";

        public NehtaSignedXml(XmlDocument document): base(document)
        {}

        /// <summary>
        /// Gets the element with the ID value.
        /// </summary>
        /// <param name="document">SOAP document.</param>
        /// <param name="idValue">ID value.</param>
        /// <returns>Element with the matching ID.</returns>

        public override XmlElement GetIdElement(XmlDocument document, string idValue)
        {
            // Create the namespace manager for XPathing
            string soapNs = document.DocumentElement.NamespaceURI;
            XmlNamespaceManager xnm = new XmlNamespaceManager(document.NameTable);
            xnm.AddNamespace("xml", XmlNamespace);
            xnm.AddNamespace("sp", spNamespace);
            xnm.AddNamespace("s", soapNs);

            // Find the element with the ID
            return document.DocumentElement.SelectSingleNode("//sp:signedPayloadData[@id = '" + idValue + "']", xnm) as XmlElement;
        }
    }
}