# User Management API
## Functional description
The User Management API is a web application designed for centralized user management in an organization or for customer service. It provides a set of REST API endpoints that allow various client applications to perform operations to create, read, update, and delete (CRUD) users in the database.
#### Key functions:
- **User Registration:** 
The application offers the functionality of registering new users. When a user registers, the data is validated, the password is hashing using SHA-256 and the user is saved to the database.
- **Blocking users:**
Administrators can block it. Blocked users cannot authenticate in the system and any access to it will return an error.
- **Viewing users:**
Only the administrator can view the list of all registered users. In addition to requesting a specific user by login, pagination is implemented and the administrator can request user lists. User passwords are not transmitted in the response for security reasons.
- **Authentication:**
The application implements Basic authorization. Users can authenticate by providing their credentials (login and password). Upon successful authentication, users gain access to protected resources.
- **User Roles:**
Users can be assigned to one of two roles: "User" and "Admin". Users with the "Admin" role have extended privileges and can perform additional operations such as blocking and retrieving users.
#### Technologies used: 
- **Entity Framework Core. ORM** 
- **PostgreSQL. The database**
- **AutoMapper. Library for projecting objects**
- **FluentValidation. A library for creating rules for validating input data**
- **Newtonsoft.Json. Library for working with data in Json format**
## Description of testing
The testing process for the application was performed using two main types of testing: integration testing and unit testing.
- **Integration tests:**
The integration tests were written to verify the basic functionality of the application's API, including interaction with the database and the correct processing of HTTP requests. Testing covers all the main API functions, including registering new users, blocking users, and retrieving a list of users.
- **Unit tests:**
Using frameworks for unit testing and mocking (for example, xUnit and FakeItEasy), tests were written for various classes such as repositories, services, and validators. These tests help ensure that every component of the system is working properly in isolated environments.
#### Technologies used: 
- **FakeItEasy. Library for creating fake objects** 
- **FluentAssertions. A set of extension methods for clear and simple implementation of testing expectations**
- **xUnit. Library for creating unit and integration tests**
## Recommendations for launching the application and conducting testing
- **Launching the WEB Application API:**
To successfully launch the application, you need to start the PostgreSQL server and change the connection string to the current one in the appsettings.json file. The application will create a database on its own and fill it with the initial data. 

It is important to note that the application does not implement the functionality of adding an administrator account, as it adds it itself during initialization. This behavior will not allow you to add another administrator and will ensure the security of access to the system.
- **Launching integration tests:**
The integration tests are located in the Integration folder. At the time of running the tests, the database should store the administrator account and the default user (they are added automatically when the application is first launched).

Testing proceeds as follows: first, the WEB API application is launched, and then the tests are run sequentially. In other words, the application must be running at the time of testing the WEB API.
- **Running unit tests:**
The integration tests are located in the Unit folder. These tests do not require preparation and can be run at any time.
