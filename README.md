# SignReferences
*Because open source sometimes causes CS1577 to anger strong naming fans.*

## How to use

Install the [NuGet package](https://www.nuget.org/packages/SignReferences/) on the project(s) where you need all references to be signed.  
It can be installed from Visual Studio NuGet packager or from NuGet console using `Install-Package SignReferences`.

## How it works

At build time, it simply checks for references, and signs in-place the ones who are unsigned.
This works very well with NuGet packages but also for assemblies referenced directly.

Explicit `.snk` files can be specified, simply add a file named `SignReferences.txt` at the root of the project.
This file contains lines in the form of:  
`<assemblyFileName>: <snkFile>`  
Wildcards are allowed, for example:  
`Stuff.*: stuff.snk`  
`Other.*: other.snk`  
Assemblies not matching names provided in list will be signed with a default random-generated snk.
Snk file names are relative to project root.


## Shall I be happy?

Yes. Probably.

