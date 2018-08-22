#!/bin/bash
# start_docker.sh

/bin/bash /src/create_test_report.sh

dotnet /app/Web.Services.dll
