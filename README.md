🛒 Bulky – E-Commerce Web Application (ASP.NET Core MVC)
🚀 Overview
Bulky is a full-featured e-commerce platform built with ASP.NET Core MVC.
It offers a complete solution for both administrators and customers, featuring product management, user authentication, shopping cart functionality, and a responsive UI built with Bootstrap.

✨ Key Features
🛍️ Product Management
Admin panel to create, read, update, and delete products and categories
Support for multiple pricing tiers (regular price, bulk discounts)
Product image upload and storage configuration
Category-based product organization

🛒 Customer Shopping Experience
Session-based shopping cart system

User registration and secure login

Order history for authenticated users

Responsive product catalog using Bootstrap 5

🔐 Authentication & Authorization
ASP.NET Identity-based registration and login

Role-based access control (Admin / Customer)

Email confirmation on registration

Protected routes using authorization attributes

🛠️ Technology Stack
Backend
ASP.NET Core 8.0 (MVC Architecture)
Entity Framework Core
Repository Pattern + Unit of Work
ASP.NET Identity for authentication and role management

Frontend
Razor Views (server-side rendering)
Bootstrap 5 for responsive design
jQuery and DataTables for enhanced admin UI

Database
SQL Server (configurable to other providers)

📁 Project Structure Overview
Areas/Admin: Admin-specific controllers and views
Areas/Customer: Customer-facing pages and logic
Data/DataAccess: Repository implementations
Models: Domain models and entities
Utilities: Constants, helpers, and static definitions
Views: Shared Razor Views
wwwroot: Static assets like CSS, JS, and images

🚀 Getting Started
Prerequisites
.NET 8 SDK
SQL Server (or preferred database)
Visual Studio 2022 or Visual Studio Code

Installation Steps
Clone the repository:
https://github.com/M-Mongy/BulkyWebApp.git
Configure the database connection in appsettings.json

Apply EF Core migrations:
dotnet ef database update

Run the application:
dotnet run or through Visual Studio

🔧 Configuration
Configure email provider in appsettings.json for email confirmation
Seed default admin credentials via the Seed method in the initializer
Adjust product image storage path in the wwwroot folder

🤝 Contributing
Contributions are welcome!
To contribute:
Fork the repository
Create a feature branch
Commit your changes
Push and open a Pull Request

📄 License
Distributed under the MIT License.
See the LICENSE file for details.

📧 Contact
Your Name – your.email@example.com
GitHub Repo: https://github.com/yourusername/Bulky

🙏 Acknowledgments
Microsoft ASP.NET Documentation
Bootstrap
Entity Framework Core Docs
And all open-source contributors

