# HMS Webservice API Documentation


HMS currently allows both GET and POST calls to the API. All output from the HMS API is Json. Primary difference between the GET and POST function is how the data location is being provided. Data location/s can be indicated using latitude and longitude coordinates, where GET or POST can be used, or can by a zipped shapefile or geojson, where POST must be used. Variable or value place holders are indicated by # and are in caps.

GET parameters are set in the url after the dataset module, separated by a '&'.

POST parameters are set in the body of the POST where the header and body follow the format provided in the example at the bottom of this readme

Available Dataset Modules:  
  
| dataset    				 | baseurl    							     | GET  |	POST |
| ------------------ | ----------------------------- | ---- | ---- |
| BaseFlow			     | ...\api\WSBaseFlow\				   | yes	| yes  |
| Evapotranspiration | ...\api\WSEvapotranspiration\ | yes	|	yes  |
| HMS (Generic)		   | ...\api\WSHMS\					       | no		| yes  |
| LandSurfaceFlow		 | ...\api\WSLandSurfaceFlow\		 | yes  |	yes  |
| Precipitation		   | ...\api\WSPrecipitation\		   | yes	|	yes  |
| SoilMoisture		   | ...\api\WSSoilMoisture\			 | yes	|	yes  |
| TotalFlow			     | ...\api\WSTotalFlow\			     | yes	|	yes  |
  
  
    
### BaseFlow Module

Both GET and POST calls can be made for baseflow data.  
GET BaseFlow url structure: #BASE_URL#\api\WSBaseFlow\\{parameters}  
GET BaseFlow parameters:

| parameter | option   | details    |   
| --------- | -------- | ---------- |    
| startDate	| REQUIRED | (format must be as dd-MM-yyyy) |  
| endDate	  | REQUIRED | (format must be as dd-MM-yyyy) |    
| latitude	| REQUIRED | |  
| longitude	| REQUIRED | |     
| source		| REQUIRED | (available baseflow sources include: 'NLDAS' and 'GLDAS') |      
| localTime	| OPTIONAL | ('true' or 'false' value, used to indicate if local or GMT time is used) |   

Example URL:  
"https:\\localhost:50052\api\WSBaseFlow\startDate=01-01-2010&endDate=01-01-2011&latitude=33&longitude=-81&source=NLDAS"  
  
POST BaseFlow url structure: #BASE_URL#\api\WSBaseFlow\  
POST parameters are provided within the body of the message, using the stated POST header/body format. A file or a lat/lon value must be provided.  
POST BaseFlow parameters: 

| parameter | option | details |  
| --------- | ------ | ------- |
| file | OPTIONAL | (.zip or .json accepted, Content-Type in the body must be changed to application/x-zip-compressed or application/json depending on the type)  
|	startDate | REQUIRED | (format must be as dd-MM-yyyy)  
|	endDate | REQUIRED | (format must be as dd-MM-yyyy)  
|	latitude | OPTIONAL | (must be provided if no zipped shapefile or geojson is given)  
|	longitude | OPTIONAL | (must be provided if no zipped shapefile or geojson is given)  
|	source | REQUIRED	| (available baseflow sources include: 'NLDAS', 'GLDAS', and 'Daymet')  
|	localTime | OPTIONAL | ('true' or 'false' value, used to indicate if local or GMT time is used)  

Refer to POST Example for POST baseflow call example.


### Evapotranspiration Module  

Both GET and POST calls can be made for evapotranspiration data.  
GET Evapotranspiration url structure: #BASE_URL#\api\WSEvapotranspiration\\{parameters}  
GET Evapotranspiration parameters:  

  |parameter | option | details
  |--------- | ------ | -------
  |	startDate | REQUIRED | (format must be as dd-MM-yyyy)		
  |	endDate | REQUIRED | (format must be as dd-MM-yyyy)		
  |	latitude | REQUIRED |  									
  |	longitude | REQUIRED |  
  |	source | REQUIRED | (available evapotranspiration sources include: 'NLDAS' and 'GLDAS')
  |	localTime | OPTIONAL | ('true' or 'false' value, used to indicate if local or GMT time is used)

Example URL:
    |https:\\localhost:50052\api\WSEvapotranspiration\startDate=01-01-2010&endDate=01-01-2011&latitude=33&longitude=-81&source=NLDAS"  

POST Evapotranspiration url structure: #BASE_URL#\api\WSEvapotranspiration\  
POST parameters are provided within the body of the message, using the stated POST header/body format. A file or a lat/lon value must be provided.  
POST Evapotranspiration parameters:  
  
  |parameter | option | details
  |--------- | ------ | -------
  |	file | OPTIONAL | (.zip or .json accepted, Content-Type in the body must be changed to application/x-zip-compressed or application/json depending on the type)
  |	startDate | REQUIRED | (format must be as dd-MM-yyyy)
  |	endDate | REQUIRED | (format must be as dd-MM-yyyy)
  |	latitude | OPTIONAL | (must be provided if no zipped shapefile or geojson is given)
  |	longitude | OPTIONAL | (must be provided if no zipped shapefile or geojson is given)
  |	source | REQUIRED | (available evapotranspiration sources include: 'NLDAS', 'GLDAS', and 'Daymet')
  |	localTime	| OPTIONAL | ('true' or 'false' value, used to indicate if local or GMT time is used)

Refer to POST Example for POST evapotranspiration call example.  

 
### HMS Module (Generic)  

Only POST calls can be made for hms data.  
POST HMS url structure: #BASE_URL#\api\WSHMS\  
POST parameters are provided within the body of the message, using the stated POST header/body format. A file or a lat/lon value must be provided.  
POST HMS parameters:

  |parameter | option | details
  |--------- | ------ | -------
  |	dataset | REQUIRED | (can get 'BaseFlow', 'Evapotranspiration', 'LandSurfaceFlow', 'Precipitaiton', 'SoilMoisture', 'TotalFlow')
  |	file | OPTIONAL | (.zip or .json accepted, Content-Type in the body must be changed to application/x-zip-compressed or application/json depending on the type)
  |	startDate | REQUIRED | (format must be as dd-MM-yyyy)
  |	endDate | REQUIRED | (format must be as dd-MM-yyyy)
  |	latitude | OPTIONAL | (must be provided if no zipped shapefile or geojson is given)
  |	longitude | OPTIONAL | (must be provided if no zipped shapefile or geojson is given)
  |	source | REQUIRED | (refer to the module of the dataset chosen for details of available sources)
  |	localTime	| OPTIONAL | ('true' or 'false' value, used to indicate if local or GMT time is used)
  |	layers | OPTIONAL | (required for SoilMoisture, defaults to layer 0 -> 0-10mm)

Refer to POST Example for POST HMS call example.
 
 
###LandSurfaceFlow Module  

Both GET and POST calls can be made for landsurfaceflow data.  
GET LandSurfaceFlow url structure: #BASE_URL#\api\WSLandSurfaceFlow\\{parameters}  
GET LandSurfaceFlow parameters:  

  |parameter | option | details
  |--------- | ------ | -------
  |	startDate | REQUIRED | (format must be as dd-MM-yyyy)		
  |	endDate | REQUIRED | (format must be as dd-MM-yyyy)		
  |	latitude | REQUIRED	| 							
  |	longitude | REQUIRED | 
  |	source | REQUIRED | (available landsurfaceflow sources include: 'NLDAS' and 'GLDAS')
  |	localTime | OPTIONAL | ('true' or 'false' value, used to indicate if local or GMT time is used)

Example URL:
	"https:\\localhost:50052\api\WSLandSurfaceFlow\startDate=01-01-2010&endDate=01-01-2011&latitude=33&longitude=-81&source=NLDAS"

POST LandSurfaceFlow url structure: #BASE_URL#\api\WSLandSurfaceFlow\  
POST parameters are provided within the body of the message, using the stated POST header/body format. A file or a lat/lon value must be provided.  
POST LandSurfaceFlow parameters:  

  |parameter | option | details
  |--------- | ------ | -------
  |	file | OPTIONAL | (.zip or .json accepted, Content-Type in the body must be changed to application/x-zip-compressed or application/json depending on the type)
  |	startDate | REQUIRED | (format must be as dd-MM-yyyy)
  |	endDate | REQUIRED | (format must be as dd-MM-yyyy)
  |	latitude | OPTIONAL | (must be provided if no zipped shapefile or geojson is given)
  |	longitude | OPTIONAL | (must be provided if no zipped shapefile or geojson is given)
  |	source | REQUIRED | (available landsurfaceflow sources include: 'NLDAS', 'GLDAS', and 'Daymet')
  |	localTime | OPTIONAL | ('true' or 'false' value, used to indicate if local or GMT time is used)

Refer to POST Example for POST landsurfaceflow call example.  
 
 
### Precipitaiton Module

Both GET and POST calls can be made for precipitation data.  
GET Precipitation url structure: #BASE_URL#\api\WSPrecipitation\\{parameters}  
GET Precipitation parameters:  

  |parameter | option | details
  |--------- | ------ | -------
  |	startDate | REQUIRED | (format must be as dd-MM-yyyy)		
  |	endDate | REQUIRED | (format must be as dd-MM-yyyy)		
  |	latitude | REQUIRED | 							
  |	longitude | REQUIRED | 
  |	source | REQUIRED | (available precipitation sources include: 'NLDAS', 'GLDAS', and 'Daymet')
  |	localTime | OPTIONAL | ('true' or 'false' value, used to indicate if local or GMT time is used)

Example URL:
	"https:\\localhost:50052\api\WSPrecipitation\startDate=01-01-2010&endDate=01-01-2011&latitude=33&longitude=-81&source=NLDAS"  

POST Preciptitation url structure: #BASE_URL#\api\WSPrecipitation\  
POST parameters are provided within the body of the message, using the stated POST header/body format. A file or a lat/lon value must be provided.  
POST Precipitation parameters:  

  |parameter | option | details
  |--------- | ------ | -------
  |	file | OPTIONAL | (.zip or .json accepted, Content-Type in the body must be changed to application/x-zip-compressed or application/json depending on the type)
  |	startDate | REQUIRED | (format must be as dd-MM-yyyy)
  |	endDate | REQUIRED | (format must be as dd-MM-yyyy)
  |	latitude | OPTIONAL | (must be provided if no zipped shapefile or geojson is given)
  |	longitude | OPTIONAL | (must be provided if no zipped shapefile or geojson is given)
  |	source | REQUIRED | (available precipitation sources include: 'NLDAS', 'GLDAS', and 'Daymet')
  |	localTime | OPTIONAL | ('true' or 'false' value, used to indicate if local or GMT time is used)

Refer to POST Example for POST precipitation call example.  
 
 
### SoilMoisture Module  

Both GET and POST calls can be made for soilmoisture data. SoilMoisture data depth can be specified by using an appropriate layer number, use the chart bellow to select desired depth from the selected source:  

  |Depth |	NLDAS layer # | GLDAS layer #
  |----- | ------------- | -------------
  |0-10mm | 0 | 0
  |10-40mm | 1 | 1
  |40-100mm | 2 | 2
  |100-200mm | 3 | 
  |0-100mm | 4	 | 3
  |0-200mm | 5 | 

GET SoilMoisture url structure: #BASE_URL#\api\WSSoilMoisture\\{parameters}  
GET SoilMoisture parameters:  
  
  |parameter | option | details 
  |--------- | ------ | -------
  |	startDate | REQUIRED | (format must be as dd-MM-yyyy)		
  |	endDate | REQUIRED | (format must be as dd-MM-yyyy)		
  |	latitude | REQUIRED | 							
  |	longitude | REQUIRED | 
  |	source | REQUIRED | (available soilmoisture sources include: 'NLDAS' and 'GLDAS')
  |	localTime | OPTIONAL | ('true' or 'false' value, used to indicate if local or GMT time is used)
  |	layers | OPTIONAL | (defaults to 0, multiple layers can be selected by adding the layer number to the list. Example: 045 for NLDAS is for layers 0-10mm, 0-100mm and 0-200mm )

Example URL:
	"https:\\localhost:50052\api\WSSoilMoisture\startDate=01-01-2010&endDate=01-01-2011&latitude=33&longitude=-81&source=NLDAS&layers=0123"

POST SoilMoisture url structure: #BASE_URL#\api\WSSoilMoisture\  
POST parameters are provided within the body of the message, using the stated POST header/body format. A file or a lat/lon value must be provided.  
POST SoilMoisture parameters:  

  |parameter | option | detials
  |--------- | ------ | -------
  |	file | OPTIONAL | (.zip or .json accepted, Content-Type in the body must be changed to application/x-zip-compressed or application/json depending on the type)
  |	startDate | REQUIRED | (format must be as dd-MM-yyyy)
  |	endDate | REQUIRED | (format must be as dd-MM-yyyy)
  |	latitude | OPTIONAL | (must be provided if no zipped shapefile or geojson is given)
  |	longitude | OPTIONAL | (must be provided if no zipped shapefile or geojson is given)
  |	source | REQUIRED | (available soilmoisture sources include: 'NLDAS' and 'GLDAS')
  |	localTime | OPTIONAL | ('true' or 'false' value, used to indicate if local or GMT time is used)
  |	layers | OPTIONAL | (defaults to 0, multiple layers can be selected by adding the layer number to the list. Example: 045 for NLDAS is for layers 0-10mm, 0-100mm and 0-200mm )

Refer to POST Example for POST soilmoisture call example.  

### TotalFlow Module  

Both GET and POST calls can be made for totalflow data. Total flow data is the summation of both baseflow and landsurfaceflow.  
GET TotalFlow url structure: #BASE_URL#\api\WSTotalFlow\\{parameters}  
GET TotalFlow parameters:  

  |parameter | option | details
  |--------- | ------ | -------
  |	startDate | REQUIRED | (format must be as dd-MM-yyyy)		
  |	endDate | REQUIRED | (format must be as dd-MM-yyyy)		
  |	latitude | REQUIRED | 							
  |	longitude | REQUIRED | 
  |	source | REQUIRED | (available totalflow sources include: 'NLDAS' and 'GLDAS')
  |	localTime | OPTIONAL | ('true' or 'false' value, used to indicate if local or GMT time is used)

Example URL:  
	"https:\\localhost:50052\api\WSTotalFlow\startDate=01-01-2010&endDate=01-01-2011&latitude=33&longitude=-81&source=NLDAS"  

POST TotalFlow url structure: #BASE_URL#\api\WSTotalFlow\  
POST parameters are provided within the body of the message, using the stated POST header/body format. A file or a lat/lon value must be provided.  
POST TotalFlow parameters:  

  |parameter | option | details
  |--------- | ------ | -------
  |	file | OPTIONAL | (.zip or .json accepted, Content-Type in the body must be changed to application/x-zip-compressed or application/json depending on the type)
  |	startDate | REQUIRED | (format must be as dd-MM-yyyy)
  |	endDate | REQUIRED | (format must be as dd-MM-yyyy)
  |	latitude | OPTIONAL | (must be provided if no zipped shapefile or geojson is given)
  |	longitude | OPTIONAL | (must be provided if no zipped shapefile or geojson is given)
  |	source | REQUIRED | (available totalflow sources include: 'NLDAS' and 'GLDAS')
  |	localTime | OPTIONAL| ('true' or 'false' value, used to indicate if local or GMT time is used)

Refer to POST Example for POST precipitation call example.  
 
 
### POST Example
```
POST HEADER EXAMPLE:  

Content-Type: multipart/form-data; boundary=-------------------------acebdf13572468  
User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0  
Content-Length: 13706  
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8  
Host: localhost:50052  

POST BODY EXAMPLE:  
  
---------------------------acebdf13572468  
Content-Disposition: form-data; name="fieldNameHere"; filename="catchment.zip"  
Content-Type: application/x-zip-compressed  
  
<@INCLUDE *C:\SAMPLEFILEPATH\SAMPLECATCHMET\catchment.zip*@>  
---------------------------acebdf13572468  
Content-Disposition: form-data; name="startDate"  
  
01-01-2015  
---------------------------acebdf13572468  
Content-Disposition: form-data; name="endDate"  
  
01-01-2016  
---------------------------acebdf13572468  
Content-Disposition: form-data; name="source"  
  
NLDAS  
---------------------------acebdf13572468  
Content-Disposition: form-data; name="localTime"  
  
false  
---------------------------acebdf13572468--  
```
