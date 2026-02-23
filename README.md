# 🛒 **Bulky – E-Commerce Web Application**  
### *Built with ASP.NET Core MVC, EF Core, Identity, Stripe & Facebook API*

---

## 🚀 Overview  
**Bulky** is a modern and full-featured **e-commerce platform** built using **ASP.NET Core MVC**.  
It provides separate interfaces for **administrators** and **customers**, offering a smooth and secure shopping experience.  

The platform includes:
- Product and category management  
- Secure authentication and authorization  
- Stripe integration for **online payments** 💳  
- Facebook API integration for **social login** 🔐  
- Responsive design with **Bootstrap 5**

---

## ✨ Key Features  

### 🛍️ Product & Category Management  
- Full **CRUD** operations for products and categories  
- Support for **multiple pricing tiers** (regular price, discounts, bulk pricing)  
- **Image upload** with configurable storage paths  
- Organized product structure under categories  

### 🛒 Customer Shopping Experience  
- Session-based **shopping cart system**  
- Secure **checkout and payment** using Stripe API  
- Login or signup via **Facebook OAuth**  
- **Order history** page for authenticated users  
- Responsive product catalog built with Bootstrap  

### 🔐 Authentication & Authorization  
- ASP.NET Identity for registration, login, and password management  
- **Facebook login integration** for faster sign-in  
- Role-based access control (**Admin / Customer**)  
- **Email confirmation** upon registration  
- Protected routes using `[Authorize]` attributes  

---

## 💳 Stripe Payment Integration  
- Seamless checkout experience via **Stripe API**  
- Secure and PCI-compliant payment handling  
- Configurable keys in `appsettings.json`
  
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
