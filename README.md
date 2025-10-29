# 🛒 **Bulky – E-Commerce Web Application**  
### *Built with ASP.NET Core MVC, EF Core & Identity*

---

## 🚀 Overview  
**Bulky** is a full-featured e-commerce platform built using **ASP.NET Core MVC**, designed for both administrators and customers.  
It offers product and category management, secure user authentication, shopping cart functionality, and a responsive UI built with **Bootstrap 5**.

---

## ✨ Key Features  

### 🛍️ Product Management  
- Admin panel to **create, read, update, and delete** products and categories  
- Support for **multiple pricing tiers** (e.g., regular price, bulk discounts)  
- **Image upload** and configurable storage path  
- Organized product structure by category  

### 🛒 Customer Shopping Experience  
- Session-based **shopping cart system**  
- **User registration and login** with secure identity handling  
- **Order history** page for authenticated users  
- Fully responsive **product catalog** using Bootstrap  

### 🔐 Authentication & Authorization  
- ASP.NET Identity for user registration and login  
- **Role-based access control** (Admin / Customer)  
- **Email confirmation** upon registration  
- Protected routes using `[Authorize]` attributes  

---

## 🛠️ Technology Stack  

### 🔙 Backend  
- **ASP.NET Core 8.0 (MVC)**  
- **Entity Framework Core**  
- **Repository Pattern + Unit of Work**  
- **ASP.NET Identity** for auth and role management  

### 🎨 Frontend  
- **Razor Views** (server-side rendering)  
- **Bootstrap 5**  
- **jQuery & DataTables** for enhanced admin UI  

### 🗄️ DataBase  
- **SQL Server** (can be configured to use other providers)

### 🔧 Configuration
Set up your email provider in appsettings.json for confirmation emails

Seed default admin credentials via the Seed method

Adjust product image path under wwwroot/images/products/

### 🤝 Contributing
We welcome contributions from the community!

To contribute:
Fork the repository

Create a new branch (git checkout -b feature/YourFeature)

Commit your changes (git commit -m 'Add new feature')

Push and open a Pull Request

### 📄 License
Distributed under the MIT License.
See the LICENSE file for more details.

### 📧 Contact
Mohamed Mongy – mohamedmongy96@gmail.com

GitHub Repo: https://github.com/M-Mongy/BulkyWebApp

### 🙏 Acknowledgments
Microsoft ASP.NET Core Documentation

Bootstrap

Entity Framework Core Docs

All open-source contributors and community support
