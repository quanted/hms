# HMS



###HMS Development Setup

HMS is being developed in C# using Visual Studio Community 2015 (Visual Studio 14.0), which is available for free: [visual studio download](https://www.visualstudio.com/vs/community/)

Steps to execute HMS windows form (currently being used for testing and development):  
  1. Download source code to your local machine. <https://github.com/USEPA/HMS>   
  2. Install Visual Studio Community 2015, if you do not already have the IDE   
  3. In the root of the HMS directory, open HMS.sln with Visual Studio   
  4. Within the Solution Explorer, right click on the WindowsFormsApplication1 project and select Set as StartUp Project   
  5. Start Project   
	
Additional Notes:   
&nbsp;&nbsp;&nbsp;&nbsp;HMS currently only works while running in DEBUG mode due to the location of the external files that are required for data retrieval.   
&nbsp;&nbsp;&nbsp;&nbsp;To run and test the webservices module of HMS, the WebServicesTest solution needs to be executed in addition to HMS (WebServicesTest is yet not located on GitHub).
	
	
Packages used by HMS.sln include:   
&nbsp;&nbsp;&nbsp;&nbsp;GeoAPI - v1.7.4   
&nbsp;&nbsp;&nbsp;&nbsp;NetTopologySuite - v1.14   
&nbsp;&nbsp;&nbsp;&nbsp;NetTopologySuite.IO - v1.14.0.1   
&nbsp;&nbsp;&nbsp;&nbsp;Newtonsoft.Json - v9.0.1   
&nbsp;&nbsp;&nbsp;&nbsp;ProjNet4GeoAPI - v1.3.0.4   
