# DEMO-Task Management System

The Task Management System is a web-based application designed to help teams and individuals manage their tasks efficiently. It provides an intuitive API for creating, updating, and tracking tasks, as well as assigning tasks to team members and projects.

## Key Features

- User Registration and Authentication: Users can register, log in, and access the system securely.
- Task Management: Create, view, update, and delete tasks with various attributes such as title, description, due date, priority, status, and category.
- Project Management: Organize tasks by assigning them to projects and tracking project progress.
- Team Collaboration: Assign tasks to team members, view tasks assigned to individual team members, and get notifications for task updates.
- Task Filtering and Searching: Users can filter tasks based on categories and search for specific tasks.
- Task Reminders: Automatic email reminders for upcoming task deadlines.
- Role-Based Access Control: Different user roles (admin, manager, team member) have varying levels of access and permissions.

## Technologies Used

- ASP.NET Core: Backend framework for building web applications.
- Entity Framework Core: Object-relational mapper for managing the database.
- Identity Framework: For user registration, authentication, and role management.
- Microsoft SQL Server: Database management system to store task and user data.
- MailKit: For sending email notifications.
- JSON Web Tokens (JWT): For secure authentication and authorization.
- Git: Version control system for collaboration and code management.

## Getting Started

To run the Task Management System on your local machine, follow the instructions to set up the development environment, database, and application configuration.

1) Prerequisites
.NET 6 SDK
Microsoft SQL Server (Express or full version) installed and running

2) Installation
- Clone the Repository.
- Configure the Database Connection:
Open the appsettings.json file in the project's root directory.
Update the DefaultConnection string with the appropriate connection details for your SQL Server instance.
Make sure to replace the values with your own.
- Build and Run the Application.

