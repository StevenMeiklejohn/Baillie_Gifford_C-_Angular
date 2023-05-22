# Introduction to C#.


### Overview

C# vs .Net
CLR (Common Language Runtime)
Architecture of .Net Applications
First C# app.

### C# Vs .Net

One of the most common questions asked when learning C# is 'what is the differnce between C# and .Net?'.
In a nutshell, C# is a programming language while .Net is a framework used for building windows applications. Other languages (such as F#) can be used with the .Net framework.

The .Net framework consists of two components.
1. The CLR (common Language Runtime)
2. Class Library.

Lets look at these in a bit more detail;
1. The CLR

Prior to the invention of C#, languages like C and C++ were compiled into machine code directly. This however meant that the compiled code could not be run a computer using different or a different processor architecture (for example).
With C# it was decided mirror the strategy employed by Java where the code is converted to IL Code (Intermediate Language Code) which is not tied to a specific architecture or operating system.
The IL Code then needs to be converted to native code appropriate for the system archetecture/operating system. This is the job of the CLR.

<img src="clr.jpg">

czzx cz 