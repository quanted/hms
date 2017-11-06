# HMS



### HMS Development Setup

Hydrologic Micro Services, HMS, is a suite of micro-services available through an API documented with Swagger.
HMS is being develop in C# .NET Core 2 with Visual Studio 2017 (Visual Studio Community 2017 is available for free): [visual studio download](https://www.visualstudio.com/downloads/)

Steps to create development enviornment:
  1. Download or fork source code from github. [HMS source](https://github.com/quanted/hms_backend.git)
  2. In Visual Studio, open up the solution file (HMS.sln)
  3. Set the Web.Services as StartUp Project
  4. Run HMS

### Current HMS data modules:   
  - Evapotranspiration   
    1. GLDAS.cs   
    2. NLDAS.cs   
  - Precipitation   
    1. Daymet.cs   
    2. GLDAS.cs   
    3. NCDC.cs   
    4. NLDAS.cs   
    5. WGEN.cs   
  - SoilMoisture   
    1. GLDAS.cs   
    2. NLDAS.cs   
  - Solar   
    1. GCSolar.cs   
  - SubSurfaceFlow   
    1. GLDAS.cs   
    2. NLDAS.cs   
  - SurfaceRunoff   
    1. CurveNumber.cs (in development)   
    2. GLDAS.cs   
    3. NLDAS.cs   
  - Temperature   
    1. Daymet.cs   
    2. GLDAS.cs   
    3. NLDAS.cs   
	
			
Packages used by HMS.sln include:   
&nbsp;&nbsp;&nbsp;&nbsp;Accord.Statistics - v3.7.0  
&nbsp;&nbsp;&nbsp;&nbsp;Newtonsoft.Json - v10.0.3   
&nbsp;&nbsp;&nbsp;&nbsp;Swashbuckle.AspNetCore.Examples - v2.3.1   
&nbsp;&nbsp;&nbsp;&nbsp;Swashbuckle.AspNetCore.Swagger - v1.0.0   
&nbsp;&nbsp;&nbsp;&nbsp;Swashbuckle.AspNetCore.SwaggerUi - v1.0.0  
