# HMS

[![Build Status](https://travis-ci.org/quanted/hms.svg?branch=dev)](https://travis-ci.org/quanted/hms)

### Description
Hydologic Micro Services (HMS) is a collection of services for collecting hydrologic data and executing models that are accessible through a REST API. HMS is written in C#, .netcore2, and developed using [Visual Studio 2017](https://www.visualstudio.com/downloads/). 

### Development Setup
Download or fork source from [github](https://github.com/quanted/hms.git).
Application is excuted from the Web.Services project.
For running HMS in docker, have docker installed for your development platform. Download available [here](https://docs.docker.com/install/).

### Accessing Endpoints
All endpoints have been documented using [swagger](https://swagger.io/), implemented using [swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) which is a .netcore2 implementation of swagger. For locally development, Startup.cs does not need to have 'HMSWS' prepended to the SwaggerEndpoint. The prepend is neccessary for server deployment on an IIS server, where the application is deployed to a site called HMSWS.

### Using Docker
Requires docker to be installed for your development platform, download available [here](https://docs.docker.com/install/). Docker will require access to a shared drive and a network adapter that has internet access. Visual Studio will also need the docker support plugin.

#### Build
The docker container and image for HMS can be build using either the CLI for docker or docker-compose. From Visual Studio, select docker-compose and set as Startup Project. Docker can now be an option for running the solution. Running with Docker will build from the dockerfile in Web.Services, which can be found [here](https://github.com/quanted/hms/blob/dev/Web.Services/Dockerfile), and construct a docker container that will provide the same functionality as a web server. The same process can be done from the command line using 'docker' pointing at the dockerfile or 'docker-compose' pointing at the docker-compose.yml. Once the container is up and running, the endpoints will be accessible through the localhost and the port that was assigned to the docker container, which can be found by running 'docker ps'. 
