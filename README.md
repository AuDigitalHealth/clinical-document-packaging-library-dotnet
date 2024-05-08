# Introduction
All HL7® CDA® documents need to be packaged according to the packaging specification [CDAP] before they can be transmitted to the personally controlled electronic health record (PCEHR) system or P2P (provider to provider) with all referenced attachments and digitally signed by a NASH organisation certificate.

# Content
The Clinical Document Packaging Library (previously called the CDA Packaging Library) simplifies the development process by providing a sample implementation of how to package CDA® documents (and any attachments) following the NEHTA specifications. There are two specifications:
- a logical specification [CP] and;
- a technical specification [CDAP] with a number of profiles.

For PCEHR and P2P, Section 2.4 (“Signed CDA package”) of the technical specification is the only supported profile as per the Clinical Documents Common Conformance Profile [CCP], which simplifies any implementation as there is a consistent approach.

A CDA® package is a ZIP file which contains a number of files and directories that must exist.

# Project
This is a software utility library for creating CDA packages using the XDM profile using .NET.

This is documented in "A Basis Repository Interface for PCEHR and Wave Site SEHRs" solution design.

# Setup
- To build and test the distributable package, Visual Studio must be installed.
- Load up the CDAPackage.sln solution file.
- For documentation on the CDAPackage library, refer to Help/Index.html.

# Solution
The solution consists of three projects:

CDAPackage: The 'Nehta.VendorLibrary.CDAPackage' project contains the utility library for
packaging CDA documents using the XDM profile.

CDAPackage.Sample: Sample code for the CDAPackage library.

Common: The 'Nehta.VendorLibrary.Common' project contains helper libraries common across
all NEHTA vendor library components.

Nehta.Xsp: The 'Nehta.Xsp' project contains helper implementations of the Xml Secured Payload Profiles (XSPP).

# DotNetZip
An open source 3rd party zip library has been used in this project for the purposes of compressing files using the zip format. More information is available here: http://dotnetzip.codeplex.com/

# Building and using the library
The solution can be built using 'F6'. 

# Library Usage
Documentation can be found in the sample project.

# Licensing
See [LICENSE](LICENSE.txt) file.
