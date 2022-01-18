# Education Management Lab - Backend internship project Spring 2022

During this internship project we will build an education management system and laborate with different principles, patterns and practices. We will also look into the IMS LTI standard. 

## Fork & Pull Request Workflow

1. Create your own fork.
2. Clone the fork to your computer.
3. Create a new development branch on your fork.
4. Do all coding in your development branch.
5. When done, merge upstream/master and rebase your development branch.
6. If appropriate, squash your commits.
7. Make a pull request.

Read more about Fork & Pull Request Workflow  [here](https://gist.github.com/Chaser324/ce0505fbed06b947d962).

## Solution description

### Core

The Core project contains the core business logic and should have minimal external dependencies. It should not depend on any infrastructure.

### EFRepository

The EFRepository project is a collection-like interface to the data layer that persists the entities using Entity Framework. The project contains a Unit of Work class that expose SaveChanges so you can batch up your work before you commit it.

### Api

The Api project is a public application programming interface. The API endpoints are described by the OpenAPI specification and Swagger UI.

### Web

The Web project is a web interface for education management.