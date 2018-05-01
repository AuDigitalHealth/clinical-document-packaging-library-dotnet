# CDA Packagin Library

This is a software utility library for creating CDA packages using the XDM profile
using .NET.

This is documented in "A Basis Repository Interface for PCEHR and Wave Site SEHRs" 
solution design.


Setup
=====

- To build and test the distributable package, Visual Studio 2008 must be installed.
- Load up the CDAPackage.sln solution file.
- For documentation on the CDAPackage library, refer to Help/Index.html.



Solution
========

The solution consists of three projects:

CDAPackage: The 'Nehta.VendorLibrary.CDAPackage' project contains the utility library for
packaging CDA documents using the XDM profile.

CDAPackage.Sample: Sample code for the CDAPackage library.

Common: The 'Nehta.VendorLibrary.Common' project contains helper libraries common across
all NEHTA vendor library components.

Nehta.Xsp: The 'Nehta.Xsp' project contains helper implementations of the Xml Secured Payload Profiles (XSPP).


DotNetZip
=========

An open source 3rd party zip library has been used in this project for the purposes of 
compressing files using the zip format. More information is available here:
	http://dotnetzip.codeplex.com/


Building and using the library
==============================

The solution can be built using 'ctrl-shift-b'. The compiled assembly can then be referenced
where the clients will be available.


Client Usage
============

Documentation can be found inline.

  
Licensing
=========
See [LICENSE](LICENSE.txt) file.
