Saga, formerly known as the saga revised codename. Is the new and improved saga (we believer) this project, the 
new system is less hardcoded based and more on input data.

The data that we provide is official data that we mainly extract throughout a gigantic database of packetlogs. 
The numbers of players on the official servers have been decreased and so are our packetlogs so the data isn't entirly 
trustworty to be compatible with latest calculations.

Getting started:

if you want to compiled saga for the first time we suggest you to setup MSBuild as shown here
http://en.csharp-online.net/MSBuild:_By_Example%E2%80%94Integrating_MSBuild_into_Visual_Studio. Then use the 
'Build.Development.Proj' build file to compile all required depend projects/solutions and default plugins.

To compile using a bat file use this line to compile:
"%WINDIR%\Microsoft.NET\Framework\v3.5\MSBuild.exe" Build.Developement.proj

Known issues:

No lua command to add a new weapon to your inventory. We do have all the packets and know how to use
them however.
	
There is a targeting problem when casting emoticons on other people. Have to debug where this goes wrong.
To debug the server, run the server from the binairy folder, in Visual Studio -> Tools -> Attach to process -> Saga.Map.Exe. 
Set a break point on the function where it does the offensive attack, and use the intermediate window to check which expression
evaluated as false.
	
AI is still a bit dumb. We need to figure out a way to read out all the height information for closed rooms with multiple height
planes in order to fix this. This will take a long amount of time so bear with the dumb ai for now.
	
If the computer is extremly stressed loading the lua quest files ad-hoc can cause problems. Lua reads the files ASCII encoding
so if you save it with unicode encoding this causes issues also. A fix could be to preload all quest files and add a reload command.
	
Mysql <5.1 is not supported. It will never be supported either. However both Mysql 6.x and 5.1 are supported.
	
The server has never been tested under a stressfull envirimont and mono. Be aware to expect bugs in that corner or even 
uncompabillity.