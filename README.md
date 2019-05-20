# HMS

[![Build Status](https://travis-ci.org/quanted/hms.svg?branch=dev)](https://travis-ci.org/quanted/hms)    [![Codacy Badge](https://api.codacy.com/project/badge/Grade/41e76175cb5a42bab24562c342f396a2)](https://www.codacy.com/app/dbsmith88/hms?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=quanted/hms&amp;utm_campaign=Badge_Grade)

### Description
Hydologic Micro Services (HMS) is a collection of services for retrieving hydrologic data and executing models that are accessible through a REST API. HMS is written in .NET Core 2 and developed using [Visual Studio 2017](https://www.visualstudio.com/downloads/). 

### Development Setup
Download or fork source from [github](https://github.com/quanted/hms.git). The Application is excuted from the Web.Services project. There are three methods for running the application:
   1. IIS: Directly through IIS as a .NET Core 2 web application (creating a publish package and importing into IIS)
   2. Docker: Web.Services contains a Dockerfile that can be used to create a docker container from Microsoft's dotnet:2.1-sdk image. The SDK image is used to include the packages required for unit testing.
   3. docker-compose: The hms project root directory contains docker-compose.yml which can be used to create the HMS Docker container in a multi-container context. The docker-compose service for HMS could also be copied into another docker-compose.yml (the quanted/qed docker-compose.yml provides a detailed example).

### Accessing Endpoints
All endpoints have been documented using [swagger](https://swagger.io/), implemented with [swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) which is a .NET Core 2 implementation of swagger. The swagger endpoint for local development can be found at "http://localhost:PORT/swagger", where the PORT is designated from Visual Studio 17 or the docker daemon (which can be found from the command line with "docker ps").

### Using Docker
To use docker, docker needs to be installed for your development platform, download available [here](https://docs.docker.com/install/). Docker will require access to a shared drive and a network adapter that has internet access. Using Docker in Visual Studio will also need the docker support plugin.

#### Build
The docker container and image for HMS can be build using either the CLI for docker or docker-compose. From Visual Studio, select docker-compose and set as Startup Project. Docker can now be an option for running the solution. Running with Docker will build from the dockerfile in Web.Services, which can be found [here](https://github.com/quanted/hms/blob/dev/Web.Services/Dockerfile), and construct a docker container that will provide the same functionality as a web server. The same process can be done from the command line using 'docker' using the dockerfile or 'docker-compose'. Once the container is up and running, the endpoints will be accessible through the localhost and the port that was assigned to the docker container'.

#### Data Request Flow Diagram
![HMS Data Request Flow Diagram](https://github.com/quanted/hms/blob/dev/hms_stack%20_flow_diagram.png)

#### Architecture Diagram
![HMS Architecture Diagram](https://github.com/quanted/hms/blob/dev/hms_architecture_diagram_docker.png)
