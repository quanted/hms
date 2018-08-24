#!/bin/sh
# Only one test project can be run at a time, output is set to a trx file
# Output is moved to the /src/test_reports/raw_trx_files directory and renamed
# trx files in /src/test_reports/raw_trx_files are merged and a html report is generated as hms_test_report.html
# html report is written to the collected_static shared volume for docker-compose

cp /src/Web.Services/App_Data/* /src/Web.Services.Tests/bin/Release/netcoreapp2.0/

dotnet test /src/Web.Services.Tests -v q -c Release -l trx;
dotnet test /src/Precipitation.Tests -v q -c Release -l trx;
dotnet test /src/Data.Tests -v q -c Release -l trx;

mv /src/Web.Services.Tests/TestResults/*.trx /src/test_reports/raw_trx_files/web_services_tests.trx
mv /src/Precipitation.Tests/TestResults/*.trx /src/test_reports/raw_trx_files/precipitation_tests.trx
mv /src/Data.Tests/TestResults/*.trx /src/test_reports/raw_trx_files/data_tests.trx

#chmod a+x /src/test_reports/trx_report.exe
#/src/test_reports/trx_report.exe /trx:/src/test_reports/raw_trx_files /output:/src/test_reports/hms_test_report.trx /report:/src/test_reports/hms_test_report_0.html

dotnet /app/Web.Services.dll