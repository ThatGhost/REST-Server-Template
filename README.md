# REST-Template

This a template for quickly setting up any standard .NET Core API rest backend server
Easy to host, Easy to understand and Easy to extend

## Features
- Micro service model
- Dependency injection
- Migration system
- Timed jobs
- Swagger for API testing/documentation
- Mysql integration
- Basic user creation and Auth
- Testing framework

### Microservices model
This project is made with microservices in mind. You can easily create and add services and repositories in the `Services` Folder.
You will then add them to the installer in the `Installer.cs` you can find in the project folder. 

### Dependency injection
The project uses the standard DI container that comes with the .NET Core API project. This makes the learning curve very small while also providing great support
There are 2 installer files in the project.

The main installer file for the microservices - `Installer.cs` - is found in the main project folder. 
The migration installer file - `MigrationRegistry.cs` - is found in the `Migrations` folder. You register your migrations there in order of execution (top first, bottom last)

### Timed jobs
I used the Quartz library for timed jobs. This library is easy to understand and works well with a DI container. In the project there is a Example job that runs everyday at 7:30AM 
These jobs are registered in the `JobTriggers.cs` file in the Core folder (feel free to move this). Feel free to read the documentation [here](https://www.quartz-scheduler.net/documentation/)
