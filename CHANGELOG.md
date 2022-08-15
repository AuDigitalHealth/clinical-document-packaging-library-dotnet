### Change Log/Revision History

No version change
Updated DotNetZip from 1.9.1.8 to 1.11.0 due to a security vulnerability (High severity). 

1.6.0
=====
Added support for netstandard2.0

1.5.4
---------
Tidied up code using nuget for ionic and removing code not required (eg Bouncy Castle).
Updated example implementation

1.5.3
---------
Added override method "GetIdElement" to remove the NCNAME test rather than capturing "Malformed reference element."
This then allows for signedXml.CheckSignature to still validate the signature.
New file added: NehtaSignedXml.cs

1.5.2
---------
Will only throw an exception if not the "Malformed reference element." - future proof code

1.5.1
---------
Linked to 1.5.0, have updated the qualifier of the reference to 'Id_' to make it clearer
Also due to signature files already generated, have removed validation if reference starts with a number.
(to protect the library against MS Security Patched PCs)

1.5.0
---------
The below files have a Guid.NewGuid().ToString() which occasionally generates a GUID that starts with a number.
This causes a "Malformed reference element." when signedXml.ComputeSignature() is called.
Fixed by adding an alpha char to the beginning of the referenceid
- SignedContainerProfileService.cs
- XmlSignatureProfileService.cs

1.4.0
---------
* Upgraded to VS2010.

1.3.7
---------
* Updated signature creation to use UTC time.
- Removed unused artefacts leftover from XDM CDA package.
- Removed GenerateReadMe() function from CDAPackageUtility.

1.3.6
---------
* Fixed issue with loading signature XMLs containing byte order mark.

1.3.5
---------
* Fixed an issue where "SHA1" is expected instead of "SHA-1" for integrity check algorithm
in CDA documents.

1.3.4
---------
* CDA package extraction changed to allow approver IDs specified as a relative URI.

1.3.3
---------
* "Preserve whitespace" set to true when processing signature files to fix an
issue with validating certain signatures.

1.3.2
---------
* Fixed an issue with importing zip files created using Java.

1.3.1
---------
* Packages consist of a discrete ZIP entry for all directories in the ZIP 
hierarchy. For example, adding the following file:

- ./IHE_XDM/SUBSET01/CDA_ROOT.XML

This will produce the following ZIP entries:

- IHE_XDM/
- IHE_XDM/SUBSET01/
- IHE_XDM/SUBSET01/CDA_ROOT.XML

Package creation library now reflects this.

1.3
---------
* Updated library in accordance with the CDA Package specification (XDM 
representation of a Clinical Package).

1.2.1
---------
* Corrected text in index.htm file. Encoding now says "UTF-8" and not "UTF=8".

1.2.0
---------
+ Added support for base CDA packages, which are CDA packages without metadata
in the CDAPackageBase class.

1.1.3
---------
* Fixed bug where importing CDA packages with invalid document class codes
  creates a CDA package of type "Shared Health Summary". An exception is now
  correctly thrown.

1.1.2
---------
* Fixed bug where creating CDA package without attachments throws an exception.

1.1.1
---------
+ Added the ability to create a package without a root document signature.
* Updated XCN functionality to required at least the last name or identifier.
* Updated XCN functionality allow for optional assigning authority identifier.
* Updated CDA package extraction to allow for packages without a root document
  signature.


1.1.0
---------
1) Added support for document entry replace operation type.
2) Added support for document entry remove operation type:
    - provides deprecation of target document entry and additional supplied
      document entry UUIDs.
    - provides ability to set remove reason metadata.
3) Updated author person XCN ID capabilities to include expanded demographic
  information:
    - First name
    - Family name
    - Given/other names
    - Prefix
    - Suffix
4) Updated document class, type and content type code value set:
    - E-Referral concept code
    - E-Referral concept name
5) Updated date precision support for creation time and submission time:
    - yyyyMMdd
    - yyyyMMddHHmm
    - yyyyMMddHHmmss


1.0.1
-----
1) Added GetFileMimeTypes.
2) Verify signature throws ValidationException instead of ApplicationException


1.0.0
-----
Original release