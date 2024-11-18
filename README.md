# MicroservicesTask

## Setup

### RabbitMQ
1. Download the RabbitMQ installer from the official RabbitMQ website.
2. Install Erlang first by downloading it from the Erlang website.
3. Once Erlang is installed, proceed to install RabbitMQ by running the downloaded `.exe` file.
4. Run `sudo systemctl start rabbitmq-server` to start RabbitMQ.
5. Run `rabbitmq-plugins enable rabbitmq_management` to enable RabbitMQ management.

### PostgreSQL
1. Go to the PostgreSQL official download page.
2. Download the installer from EnterpriseDB.
3. Run the installer by executing the downloaded `.exe` file.
4. Setup the database and create the table.

### Visual Studio
1. Go to the Visual Studio download page.
2. Download the Community edition, setup, and install.

## Overview

### FilePublisherService
- **Controller**
    1. Injected `filePublisherService`.
    2. Created `/Publish` endpoint on which `PublishFileData` will be executed, calling the service class method and passing the input parameter as the file path.
    3. Exception handling done in case of errors.
- **Service**
    1. Set up RabbitMQ connection and channel.
    2. Declared queue and its configuration.
    3. Used iteration and reading file line by line as mentioned in the requirement.
    4. Used the `BasicPublish()` method to publish the message to the RabbitMQ queue.
    5. Closed the connection.
- **appsettings.json**
    1. Configure RabbitMQ credentials.

### DataConsumerService
- **Controller**
    1. Created `/GetFileData` endpoint to retrieve data based on the name, applied pagination, and filters.
    2. Exception handling done in case of errors.
- **Service**
    1. Set up RabbitMQ connection and channel.
    2. Declared queue and its configuration.
    3. Used iteration and reading file line by line as mentioned in the requirement.
    4. Used `StartAsync()` method to create scope for message processing, `StoreMessageInDatabase()` to store data in the database, and `BasicAck()` to return acknowledgement.
    5. Closed the connection in `StopAsync()`.
- **appsettings.json**
    1. Configure RabbitMQ credentials.
    2. Database connection string.
- **Program.cs**
    1. Added service registration for `DataConsumerService`.
    2. Added DBContext registration for the database connection.
    3. Created `DataContext` and `FileData` model, and also performed EF migration.

**Note:** Changed the port as both services were running on the same port.

## Flow:
The `FilePublisherService` takes the file path as input, from which it tries to read the file line by line. When successful, it sends the line to the `FileData` queue, to which the `DataConsumerService` is listening. Once the message appears in the queue, the `DataConsumerService` reads it, stores the data in the database, and returns the acknowledgement to the queue.
