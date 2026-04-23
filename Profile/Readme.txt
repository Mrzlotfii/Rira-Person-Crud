# Profile Management System

A simple CRUD implementation for person management using Protocol Buffers and gRPC

## Description

This project is a CRUD service for managing person information, implemented using gRPC and Protocol Buffers.

**Important Note**: gRPC is typically used for service-to-service communication, but in this project, as per task requirements, gRPC with JSON Transcoding capability is used to enable direct client access as well.

## Project Structure

### DomainModels
Domain models
- **Persons**: Contains `Person.cs` and `PersonErrors.cs`

### Features
CRUD operations using Vertical Slice Architecture
- **Persons**
  - `CreatePerson.cs` - Create new person
  - `DeletePerson.cs` - Delete person
  - `GetPerson.cs` - Get person details
  - `GetPersons.cs` - Get list of persons
  - `UpdatePerson.cs` - Update person information

### Grpc
gRPC services
- `person.proto` - gRPC contract definition

### Infrastructure
Infrastructure layer
- **Database/Store**: `PersonStore.cs` - In-Memory data storage using Dictionary with Lock for Thread-Safety
- **Services**: `PersonCrudService.cs` - CRUD service
- Configuration files: `appsettings.json`, `appsettings.Development.json`
- `Program.cs` - Application entry point

### SharedKernel
Shared components
- **Domain**
  - `BasEntity.cs` - Base entity class
  - `Error.cs` - Error handling
  - `ISnowflakeGenerator.cs` - Unique ID generator interface
  - `Result.cs` - Result pattern for operation outcomes
- **Infrastructure/Helper**
  - `IHandler.cs` - Handler interface
  - `SnowflakeGenerator.cs` - Snowflake ID generator implementation

## Technologies
- .NET 10
- gRPC with JSON Transcoding
- Protocol Buffers
- FluentValidation
- Vertical Slice Architecture
- Snowflake ID Generator
- In-Memory Store with Thread-Safe Dictionary

## Installation & Running
```bash
dotnet restore
dotnet run

The service runs on `http://localhost:5115` with support for HTTP/1.1 and HTTP/2 protocols.

## Features
- gRPC service with Reflection capability
- Automatic Handler registration via Assembly Scanning
- Request validation with FluentValidation
- In-Memory data storage with Thread-Safety
- Unique ID generation using Snowflake pattern
- JSON Transcoding support for REST-like access
