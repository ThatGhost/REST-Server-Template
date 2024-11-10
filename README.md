# REST-Template

This a template for quickly setting up any standard .NET Core API rest backend server
Easy to host, Easy to understand and Easy to extend

## Features

- Micro service model
- Dependency injection
- Migration system
- Timed jobs
- Swagger
- Mysql integration
- Basic user creation and Auth
- Testing framework

### Microservices model

This project is made with microservices in mind. You can easily create and add services and repositories in the `Services` Folder.
You will then add them to the installer in the `Installer.cs` you can find in the project folder. 

### Dependency injection

The project uses the standard DI container with the .NET Core API project. This makes the learning curve very small while also providing great support
There are 2 installer files in the project.

The main installer file for the microservices — `Installer.cs` — is in the main project folder. 
The migration installer file - `MigrationRegistry.cs` - is found in the `Migrations` folder. You register your migrations there in order of execution (top first, bottom last)

### Timed jobs

I used the Quartz library for timed jobs. This library is easy to understand and works well with a DI container. In the project, there is an Example job that runs every day at 7:30AM 
These jobs are registered in the `JobTriggers.cs` file in the Core folder (feel free to move this). Feel free to read the documentation [here](https://www.quartz-scheduler.net/documentation/)

### Swagger

Swagger is used for documentation and will automatically start when developing locally. If you close this window the application will also stop. I use Swagger as it is widely used in professional environments and is a widely supported and understood tool.
Every Endpoint in Swagger contains an `Authentication` field even when authentication is not required. This is to prevent extra boilerplate and clutter in the controller files. You can access these authentication headers using the following code.

``
  await _usersAuthenticationService.AuthenticateUser(Request.Headers);
  // Request.Headers
``

If you want to know more or modify the swagger. There are plenty of resources about said topic to be found online.

### Mysql

The Database language I chose is MySql because I am comfortable with it. It is quite easily swapped out for other languages that Dapper supports. You would just have to rewrite some queries in the user repositories and Migration code and swap out the code in the base repository.

It is merely a stylistic choice.

### Basic user creation and Auth

I added some basic functionality for this project. This comes in the form of my personally created User creation and User Auth system. The endpoints should be self explanatory. And i will not go too deep into the technical details. 
But these are the basics. 
A user has a UUID which is used as the unique identifier. This is used for Authentication -> checks against a hash created from the password hash and the email always resulting in a check that can not be manipulated by the user.

!! **Change the `GetHashSring(string inputString)` function for even better security** !!

never show this code to users

Authorization is very basic by only checking if the token belongs to the user trying to do an action but should be extended per feature or as needed.

### Testing

Tests are done using the NUnit framework from Microsoft. This is again a widely used and understood framework so there are a bunch of resources online if you had any questions. 
All the user services are tested in the `Tests` project. When u create new services it is good practice to add your unit tests immediately. The premade tests should be a good example of common practices.

## Closing word

This project has all the necessary features for a REST backend that can easily be deployed. It harnesses the power of common libraries to make it as easy to understand and pick up as possible.
If you end up using this template for any projects give this Repo a star, please ;)
