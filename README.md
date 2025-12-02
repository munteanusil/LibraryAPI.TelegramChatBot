Bookstore API Backend 

This repository contains the Backend API implementation for a modern online bookstore management system. The project offers a full set of CRUD (Create, Read, Update, Delete) functionalities for managing books, authors, and categories, including the definition of essential entity relationships.

Designed as a robust foundation, this API is the first phase of a larger project aiming for integration into an intelligent bookstore chatbot (via WhatsApp/Telegram APIs) and a possible dedicated Angular Frontend interface.

<img width="392" height="852" alt="image" src="https://github.com/user-attachments/assets/f0b3c53c-2138-4251-8cca-24de2bd3e7b9" />
<img width="575" height="676" alt="image" src="https://github.com/user-attachments/assets/ac20fce1-ee9a-4276-9a75-0d877c268fd0" />

Key Features
The API exposes endpoints for the following data management operations:

1.  Book Management (Books)
Create: Adding new books to the database.

Read: Viewing the list of available books and individual details (title, author, price, category).

Update: Updating existing book information.

Delete: Removing a book from the bookstore.

2.  Author Management (Authors)
Create: Adding new authors.

Read: Viewing the list of authors and their details.

Update: Updating an author's information.

Delete: Deleting an author (optional, only if no associated books exist).

3.  Category Management (Categories)
Create: Adding new book categories.

Read: Viewing available categories.

Update: Updating a category's name.

Delete: Deleting a category (optional, only if no associated books exist).

4.  Entity Relationships
One-to-many relationships have been defined (a book belongs to a single author/category, but an author/category can have multiple books).

Technologies and Frameworks
The project is built on the Microsoft ecosystem, leveraging modern technologies:

Backend API: .NET 8 using ASP.NET Core Web API.

Data Access: Entity Framework Core for database access and object mapping.

Database: SQL Server or SQLite (for fast/local development).

API Documentation: Swagger/OpenAPI for automatic documentation and interactive endpoint testing.

Mapping: AutoMapper (optional) for mapping between database entities and DTOs (Data Transfer Objects).

 Implementation Details and Optimizations
To ensure clean, performant, and maintainable code, the following practices have been included:

Structure: ASP.NET Core Web API project creation and initial Entity Framework Core configuration.

Controllers: Implementation of Books, Authors, and Categories controllers with the relevant CRUD methods.

Validation: Added validation (e.g., required fields) for input data.

Performance: Optimization of Entity Framework Core queries using .AsNoTracking() for read operations where necessary.

Usability: Implementation of pagination and filtering on GET list endpoints.

Testing: Endpoint testing using Postman or the Swagger interface.




 Future Vision (Possible Extensions)
This API serves as the foundation for the following next steps:

React/Angular Frontend: Developing a complete Graphical User Interface using Angular to visually interact with the bookstore data.

Authentication & Authorization: Implementing role-based security (e.g., Admin vs. User).

Chatbot Integration: Connecting the API to messaging platforms like WhatsApp or Telegram (using their dedicated APIs) to allow users to search for books, authors, and receive recommendations directly in chat.
