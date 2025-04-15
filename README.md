# 🚍 Transport Management System

A **desktop application** that enables travel agencies to **search for flights** and **book tickets** for their clients, built with a custom **RPC-based client-server architecture**.

> ✨ This system is implemented in both **Java** and **C#**, showcasing cross-platform development with consistent business logic and architecture.

---

## 📘 Project Overview

Multiple travel agencies collaborate with a central airline company to manage tourist transportation. Employees from each agency use this application to perform daily operations such as:

### 🔐 Authentication
Log in securely with a username and password. Upon successful login, users can view all available flights, including:
- Destination  
- Departure date and time  
- Airport  
- Available seats  

### 🔍 Flight Search
Search for flights by destination and departure date. Matching results will be displayed with all necessary details.

### 🎫 Ticket Booking
Buy tickets for specific flights by entering:
- Number of seats  
- Tourists’ names  

Once a ticket is purchased:
- All connected agency employees receive **real-time updates**
- Flights with no available seats are automatically **removed** from the interface

---

## 💡 Key Features

### 🖥 Client
- 🔐 **Login** with secure password hashing (`BCrypt`)
- 🔍 **Search Flights** by destination and date
- 🎫 **Book Tickets** with automatic update of seat availability
- 🔔 **Live Updates** via the Observer Pattern
- 🌐 **RPC Protocol Communication** with 2 classes Response and Request (and one enum for each)
- 💎 Modern **JavaFX GUI** / **Windows Forms**

### 🗄 Server
- 🧵 **Multithreaded processing** using `ExecutorService` or `Platform.runLater()`
- 📢 **Observer notifications** for live updates
- 🔒 **Thread-safe logic**
- 🛠️ **JDBC Repositories**
- 📋 **Logging** via Log4j
- 🔐 **Password Security** with `BCrypt`
- 🌐 **JSON** serialization/deserialization for c#
- 📋 **SQLite** for data persistance, with reading the connection string from a configuration file


## 🔎 Technical Highlights

### 🔄 Reflection
Both implementations use **reflection** to dynamically dispatch RPC calls:
- Java: `Method.invoke(...)`
- C#: `MethodInfo.Invoke(...)`

This allows for a flexible and scalable RPC system where each request type maps to a `Handle<REQUEST_TYPE>` method.

### 📡 Observer Pattern
Clients implement an observer interface:
- When a ticket is purchased, all connected clients receive real-time updates via server push notifications.

### 🏗️ MVC Architecture
The Model-View-Controller (MVC) pattern is used to separate the application into three layers:

 - Model: Manages data and business logic (e.g., flights, tickets, user authentication).

 - View: Handles the user interface (JavaFX for Java, Windows Forms for C#) (e.g. fxml - file).

 - Controller: Mediates between the model and view, handling user input and business logic (e.g. LoginController, MainController).

### 🔧 Object-Oriented Programming (OOP)
The project follows OOP principles, including:

 - Encapsulation: All data and operations are encapsulated within objects (e.g., User, Flight, Ticket).

 - Inheritance: Allows for code reusability and hierarchy management (e.g., extending base service classes).

 - Polymorphism: Dynamically handling different request types via reflection.

 - Abstraction: Simplifying complex operations (e.g., hiding low-level server details from clients).

### 🔒 Singleton Pattern
The Singleton pattern ensures that there is only one instance of key services in the system (e.g., TransportServiceImpl). This is used to manage shared resources efficiently and provide a global point of access.

### 🏰 Proxy Pattern
The Proxy pattern is used for client-server communication, where a proxy object is created to represent the server's behavior:

The client communicates with the proxy, which handles the actual RPC calls to the server. This enables flexibility and additional functionality, such as logging or security checks, without modifying the server directly.

### 🚀 Future Technologies
In the future, this system will be enhanced using advanced networking technologies, such as:

gRPC: A high-performance RPC framework that would replace the custom RPC solution in this project, offering faster communication and bi-directional streaming.

Apache Thrift: An efficient framework for cross-language services, which could improve flexibility and performance.

Additionally, I plan to develop the client and server in two different languages (c# - java) to optimize interoperability and scalability of the application.
---

