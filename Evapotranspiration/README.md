# Evapotranspiration Documentation

This document describes the input and output parameters for 14 Evapotranspiration algorithms.

### Base Input Parameters (Used by all algorithms):
| Parameter	 | Unit		    | Example  |
| -----------|------------|----------|
| Latitude	 |Degrees   	|33.893    |
| Longitude	 |Degrees	    |-83.356   |
| Start Date |YYYY-MM-DD	|2014-01-01|
| End Date	 |YYYY-MM-DD	|2014-12-31|
| Time Zone\*|String      |Eastern   |
##### \*Time zone (offset) can be calculated using Lat/Long parameters, so this field will not be needed in HMS.
-------------------------------------------------------
### 1. Hamon
##### Input Parameters:
Base Input Parameters

##### Output Table:
|Parameter			 		           |Unit	      			|Example    |
|------------------------------|------------------|-----------|
|Date				 		               |YYYY-MM-DD 			  |2014-01-01 |
|Julian Day			 		           |Integer	  			  |1		      |
|Minimum Temperature 		       |Celsius	  			  |1.23		    |
|Maximum Temperature 		       |Celsius	  			  |11.91	    |
|Mean Temperature    		       |Celsius	  			  |6.57		    |
|Hours of Sunshine   		       |Hours	  			    |9.87		    |
|Potential Evapotranspiration  |Millimeters/Day |0.0275	    |
-----------------------------------------------------------------
### 2. Priestly-Taylor
##### Input Parameters:
Base Input Parameters

|Parameter			 			 |Unit	     |Example    |
|--------------------------------|-----------|-----------|
|Elevation(Derived from Lat/Long)|Meters 	 |193.09	 |
|Albedo Coefficient		 	     |Double	 |0.23		 |

##### Output Table:
|Parameter			 		  |Unit	    	  		|Example    |
|-----------------------------|---------------------|-----------|
|Date				 		  |YYYY-MM-DD 	  		|2014-01-01 |
|Julian Day			 		  |Integer	      		|1		  	|
|Minimum Temperature 		  |Celsius	      		|1.23		|
|Maximum Temperature 		  |Celsius	      		|11.91	  	|
|Mean Temperature    		  |Celsius	      		|6.57		|
|Mean Solar Radiation   	  |Megajoules/Sq. Meter |9.87	    |
|Potential Evapotranspiration |Millimeters/Day    |0.0379	  	|
-----------------------------------------------------------------
### 3. Granger-Gray
##### Input Parameters:
Base Input Parameters

|Parameter			 			 |Unit	     |Example    |
|--------------------------------|-----------|-----------|
|Elevation(Derived from Lat/Long)|Meters 	 |193.09	 |
|Albedo Coefficient		 	     |Double	 |0.23		 |

##### Output Table:
|Parameter			 		  |Unit	    	  			|Example    |
|-----------------------------|-------------------------|-----------|
|Date				 		  |YYYY-MM-DD 	  			|2014-01-01 |
|Julian Day			 		  |Integer	      			|1		  	|
|Minimum Temperature 		  |Celsius	      			|1.23		|
|Maximum Temperature 		  |Celsius	      			|11.91	  	|
|Mean Temperature    		  |Celsius	      			|6.57		|
|Mean Solar Radiation   	  |Megajoules/Sq. Meter		|9.87	    |
|Mean Wind Speed			  |Meters/Second  			|2.34 	  	|
|Minimum Relative Humidity	  |Percentage    	  		|85.26	    |
|Maximum Relative Humidity	  |Percentage    	  		|49.80	    |
|Potential Evapotranspiration |Millimeters/Day    	|0.0115	  	|
---------------------------------------------------------------------
### 4. Penpan
##### Input Parameters:
Base Input Parameters

|Parameter			 			 |Unit	     |Example    |
|--------------------------------|-----------|-----------|
|Elevation(Derived from Lat/Long)|Meters 	 |193.09	 |
|Albedo Coefficient		 	     |Double	 |0.23		 |

##### Output Table:
|Parameter			 		  |Unit	    	  		|Example    |
|-----------------------------|---------------------|-----------|
|Date				 		  |YYYY-MM-DD 	  		|2014-01-01 |
|Julian Day			 		  |Integer	      		|1		    |
|Minimum Temperature 		  |Celsius	      		|1.23		|
|Maximum Temperature 		  |Celsius	      		|11.91	  	|
|Mean Temperature    		  |Celsius	      		|6.57		|
|Mean Solar Radiation   	  |Megajoules/Sq. Meter |9.87	    |
|Mean Wind Speed			  |Meters/Second  		|2.34 	  	|
|Minimum Relative Humidity	  |Percentage    	  	|85.26	    |
|Maximum Relative Humidity	  |Percentage    	  	|49.80	    |
|Potential Evapotranspiration |Millimeters/Day    |0.1352	  	|
-----------------------------------------------------------------
### 5. McJannett
##### Input Parameters:
Base Input Parameters

|Parameter			 			 |Unit	     		|Example    |
|--------------------------------|------------------|-----------|
|Albedo Coefficient		 	     |Double	 		|0.23		 |
|Surface Area of Lake			 |Sq. Kilometers	|0.005		 |
|Mean Depth of Lake			 |Meters		 	|0.2		 |
|Air Temperature Coefficient*	 |Double	 		|1.0		 |
\*Each month has its own Air Temperature Coefficient

##### Output Table:
|Parameter			 		  |Unit	    	  		|Example    |
|-----------------------------|---------------------|-----------|
|Date				 		  |YYYY-MM-DD 	  		|2014-01-01 |
|Julian Day			 		  |Integer	      		|1		    |
|Minimum Temperature 		  |Celsius	      		|1.23	    |
|Maximum Temperature 		  |Celsius	      		|11.91	    |
|Mean Temperature    		  |Celsius	      		|6.57	    |
|Mean Solar Radiation   	  |Megajoules/Sq. Meter |9.87	    |
|Mean Wind Speed			  |Meters/Second  		|2.34 	    |
|Minimum Relative Humidity	  |Percentage    	  	|85.26	    |
|Maximum Relative Humidity	  |Percentage    	  	|49.80	    |
|Water Temperature    		  |Celsius	      		|6.57	    |
|Potential Evapotranspiration |Millimeters/Day    |0.0490		|
-----------------------------------------------------------------
### 6. Penman Open Water
##### Input Parameters:
Base Input Parameters

|Parameter			 			 |Unit	     |Example    |
|--------------------------------|-----------|-----------|
|Elevation(Derived from Lat/Long)|Meters 	 |193.09	 |
|Albedo Coefficient		 	     |Double	 |0.23		 |

##### Output Table:
|Parameter			 		  |Unit	    	 		|Example    |
|-----------------------------|---------------------|-----------|
|Date				 		  |YYYY-MM-DD 	  		|2014-01-01 |
|Julian Day			 		  |Integer	      		|1		    |
|Minimum Temperature 		  |Celsius	      		|1.23		|
|Maximum Temperature 		  |Celsius	      		|11.91	    |
|Mean Temperature    		  |Celsius	      		|6.57		|
|Mean Solar Radiation   	  |Megajoules/Sq. Meter |9.87	    |
|Mean Wind Speed			  |Meters/Second  		|2.34 	    |
|Minimum Relative Humidity	  |Percentage    	  	|85.26	    |
|Maximum Relative Humidity	  |Percentage    	  	|49.80	    |
|Potential Evapotranspiration |Millimeters/Day    |0.0944	    |
-----------------------------------------------------------------
### 7. Penman Daily
##### Input Parameters:
Base Input Parameters

|Parameter			 			 |Unit	     |Example    |
|--------------------------------|-----------|-----------|
|Elevation(Derived from Lat/Long)|Meters 	 |193.09	 |
|Albedo Coefficient		 	     |Double	 |0.23		 |

##### Output Table:
|Parameter			 		  |Unit	    	  		|Example    |
|-----------------------------|---------------------|-----------|
|Date				 		  |YYYY-MM-DD 	  		|2014-01-01 |
|Julian Day			 		  |Integer	      		|1		    |
|Minimum Temperature 		  |Celsius	      		|1.23		|
|Maximum Temperature 		  |Celsius	      		|11.91	    |
|Mean Temperature    		  |Celsius	      		|6.57		|
|Mean Solar Radiation   	  |Megajoules/Sq. Meter |9.87	    |
|Mean Wind Speed			  |Meters/Second  		|2.34 	    |
|Minimum Relative Humidity	  |Percentage    	  	|85.26	    |
|Maximum Relative Humidity	  |Percentage    	  	|49.80	    |
|Potential Evapotranspiration |Millimeters/Day    |0.0932	    |
-----------------------------------------------------------------
### 8. Penman Hourly
##### Input Parameters:
Base Input Parameters

|Parameter			 			 |Unit	     |Example    |
|--------------------------------|-----------|-----------|
|Elevation(Derived from Lat/Long)|Meters 	 |193.09	 |
|Albedo Coefficient		 	     |Double	 |0.23		 |
|Central Longitude of Time Zone	 |Degrees 	 |75.0		 |
|Sun Angle				 	     |Degrees	 |17.2		 |

##### Output Table:
|Parameter			 		  |Unit	    	  		|Example    |
|-----------------------------|---------------------|-----------|
|Date				 		  |YYYY-MM-DD 	  		|2014-01-01 |
|Julian Day			 		  |Integer	      		|1		    |
|Hourly Temperature 		  |Celsius	      		|4.08 	    |
|Mean Solar Radiation   	  |Megajoules/Sq. Meter |9.87	    |
|Mean Wind Speed			  |Meters/Second  		|1.83 	    |
|Hourly Relative Humidity	  |Percentage    	  	|78.32 	    |
|Potential Evapotranspiration |Millimeters/Day   	|0.0009	    |
-----------------------------------------------------------------
### 9. Morton CRAE
##### Input Parameters:
Base Input Parameters

|Parameter			 			 |Unit	     |Example    |
|--------------------------------|-----------|-----------|
|Elevation(Derived from Lat/Long)|Meters 	 |193.09	 |
|Annual Precipitation			 |Millimeters|1185.9	 |
|Albedo Coefficient		 	     |Double	 |0.23		 |
|Emissivity				 	     |Double	 |0.92		 |
|Model Type				 	     |String	 |ETP		 |

##### Output Table:
|Parameter			 		  |Unit	    	  		|Example    |
|-----------------------------|---------------------|-----------|
|Date				 		  |YYYY-MM-DD 	  		|2014-01-01 |
|Julian Day			 		  |Integer	      		|1		    |
|Minimum Temperature 		  |Celsius	      		|1.23		|
|Maximum Temperature 		  |Celsius	      		|11.91	    |
|Mean Temperature    		  |Celsius	      		|6.57		|
|Mean Solar Radiation   	  |Megajoules/Sq. Meter |9.87	    |
|Minimum Relative Humidity	  |Percentage    	  	|85.26	    |
|Maximum Relative Humidity	  |Percentage    	  	|49.80	    |
|Potential Evapotranspiration |Millimeters/Day    |0.0854	    |
-----------------------------------------------------------------
### 10. Morton CRWE
##### Input Parameters:
Base Input Parameters

|Parameter			 			 |Unit	     |Example    |
|--------------------------------|-----------|-----------|
|Elevation(Derived from Lat/Long)|Meters 	 |193.09	 |
|Annual Precipitation			 |mm	 	 |1185.9	 |
|Albedo Coefficient		 	     |Double	 |0.23		 |
|Emissivity				 	     |Double	 |0.97		 |
|Zenith Albedo Coefficient 	     |Double	 |0.05		 |
|Model Type				 	     |String	 |ETP		 |

##### Output Table:
|Parameter			 		  |Unit	    	  		|Example    |
|-----------------------------|---------------------|-----------|
|Date				 		  |YYYY-MM-DD 	  		|2014-01-01 |
|Julian Day			 		  |Integer	      		|1		    |
|Minimum Temperature 		  |Celsius	      		|1.23		|
|Maximum Temperature 		  |Celsius	      		|11.91	    |
|Mean Temperature    		  |Celsius	      		|6.57		|
|Mean Solar Radiation   	  |Megajoules/Sq. Meter |9.87	    |
|Minimum Relative Humidity	  |Percentage    	  	|85.26	    |
|Maximum Relative Humidity	  |Percentage    	  	|49.80	    |
|Potential Evapotranspiration |Millimeters/Day    |0.1049	    |
-----------------------------------------------------------------
### 11. Shuttleworth-Wallace
##### Input Parameters:
Base Input Parameters

|Parameter			 			 |Unit	     								|Example     |
|--------------------------------|------------------------------------------|------------|
|Elevation(Derived from Lat/Long)|Meters 	 								|193.09  	 |
|Albedo Coefficient		 	     |Double	 								|0.23		 |
|Stomatal Resistance			 |Siemens/Reciprocal Meter					|400.0	 	 |
|Surface Resistance of Substrate |Siemens/Reciprocal Meter			 		|500.0		 |
|Ground Roughness Length		 |Meters			 						|0.02		 |
|Leaf Width				 	     |Meters			 						|0.02		 |
|Vegetation Height				 |Meters			 						|0.12		 |
|Leaf Area Indices*				 |Leaf Area/Ground Area, or  Sq.Meters/Sq.Meters|2.51		 |
\*Each month has its own Leaf Area Index (LAI)

##### Output Table:
|Parameter			 		  |Unit	    	  		|Example    |
|-----------------------------|---------------------|-----------|
|Date				 		  |YYYY-MM-DD 	  		|2014-01-01 |
|Julian Day			 		  |Integer	      		|1		    |
|Minimum Temperature 		  |Celsius	      		|1.23		|
|Maximum Temperature 		  |Celsius	      		|11.91	    |
|Mean Temperature    		  |Celsius	      		|6.57		|
|Mean Solar Radiation   	  |Megajoules/Sq. Meter |9.87	    |
|Mean Wind Speed			  |Meters/Second  		|2.34 	    |
|Minimum Relative Humidity	  |Percentage    	  	|85.26	    |
|Maximum Relative Humidity	  |Percentage    	  	|49.80	    |
|Potential Evapotranspiration |Millimeters/Day    |0.0121		|
-----------------------------------------------------------------
### 12. HSPF
##### Input Parameters:
Base Input Parameters

|Parameter			 			 |Unit	     |Example    |
|--------------------------------|-----------|-----------|
|Elevation(Derived from Lat/Long)|Meters 	 |193.09	 |
|Albedo Coefficient		 	     |Double	 |0.23		 |
|Central Longitude of Time Zone	 |Degrees 	 |75.0		 |
|Sun Angle				 	     |Degrees	 |17.2		 |

##### Output Table:
|Parameter			 		  |Unit	    	  		|Example    |
|-----------------------------|---------------------|-----------|
|Date				 		  |YYYY-MM-DD 	  		|2014-01-01 |
|Julian Day			 		  |Integer	      		|1		    |
|Hourly Temperature 		  |Celsius	      		|4.08 	    |
|Mean Solar Radiation   	  |Megajoules/Sq. Meter |9.87	    |
|Mean Wind Speed			  |Meters/Second  		|1.83 	    |
|Hourly Precipitation		  |Percentage		   	|0.171	    |
|Potential Evaporation		  |Millimeters/Day	|0.000051	|
|Hourly Relative Humidity	  |Percentage   	  	|78.32 	    |
|Potential Evapotranspiration |Millimeters/Day    |0.0009	    |
|Dew Point Temperature        |Fahrenheit    		|37.5720    |
|Cloud Coverage 			  |Okta (Percentage)    |1.00	    |
-----------------------------------------------------------------
### 13. GLDAS
##### Input Parameters:
Base Input Parameters

##### Output Table:
|Parameter			 		  |Unit	    	   			|Example    	 |
|-----------------------------|-------------------------|----------------|
|DateHour				 	  |YYYY-MM-DD HH:MM			|2014-01-01 00:00|
|Potential Evapotranspiration |Kilogram/Sq. Meter/Second|0.000013180	 |
--------------------------------------------------------------------------
### 14. NLDAS
##### Input Parameters:
Base Input Parameters

##### Output Table:
|Parameter			 		  |Unit	    	   			|Example    	 |
|-----------------------------|-------------------------|----------------|
|DateHour				 	  |YYYY-MM-DD HH:MM			|2014-01-01 00:00|
|Potential Evapotranspiration |Kilogram/Sq. Meter/Second|0.000013180	 |
--------------------------------------------------------------------------
