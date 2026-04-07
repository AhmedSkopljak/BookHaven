# 📚 BookHaven - Enterprise Inventory Management System

**BookHaven** is a high-performance E-commerce management platform built with the latest **.NET 9 SDK**. The application follows a professional **N-Tier Architecture** and implements advanced design patterns to provide a scalable and secure solution for book inventory and role-based access.

---

## 🛠️ Key Technical Features

### 🏗️ Professional Architecture
- **N-Tier Separation:** Strictly decoupled solution with four main projects: 
    - `BookHavenWeb` (UI/Presentation)
    - `BookHaven.DataAccess` (Data Persistence & Repository Logic)
    - `BookHaven.Models` (Domain Entities & Identity)
    - `BookHaven.Utility` (Static Constants & Helper Classes)
- **Repository Pattern & Unit of Work:** Centralized data access layer for clean, maintainable, and testable code.
- **Dependency Injection:** Full utilization of .NET's built-in DI container for managing service lifetimes and loose coupling.

### 🔐 Security & RBAC (Role-Based Access Control)
- **ASP.NET Core Identity:** Secure authentication and authorization system.
- **Custom Identity Attributes:** User profiles extended with fields like Name, Street Address, City, State, and Postal Code.
- **Roles Managed via Static Details (SD):** - `Admin`: Full CRUD control over categories and products.
    - `Employee`: Access to store operations and inventory management.
    - `Company`: Specialized account for B2B entities.
    - `Customer`: Catalog browsing and product viewing.

### 📦 Product & Content Management
- **Full Inventory CRUD:** Comprehensive management of Books (Products) and Categories.
- **Rich Text Editing:** Integrated **TinyMCE** for professional, formatted book descriptions.
- **Media Management:** Local image upload/deletion for book covers, managed via `wwwroot/images`.
- **Advanced UI Tables:** Using **DataTables** (Client-side) for fast searching, sorting, and pagination of products in the Admin panel.

### 🎨 User Experience (UX)
- **Interactive Feedback:** **Toastr** notifications powered by `TempData` for real-time success/error messages.
- **Smooth Modals:** **SweetAlert2** for elegant delete confirmations and interactive alerts.
- **Validations:** Robust server-side and client-side validations using .NET Data Annotations.
- **Modern UI:** Responsive design built with **Bootstrap 5** and **Bootswatch** themes.

---

## 🛠️ Tech Stack

- **Framework:** .NET 9 (C# / MVC)
- **Database:** Microsoft SQL Server (SSMS)
- **Front-end:** Razor Pages, JavaScript, jQuery, HTML5, CSS3, Bootstrap 5
- **Third-Party Plugins:** TinyMCE, SweetAlert2, Toastr, DataTables

---

## 🚀 Setup & Installation

Follow these steps to get the project running on your local machine:

### 1. Prerequisites
- Install **[.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)**.
- Install **[SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)**.

### 2. Clone the Repository```bash
git clone [https://github.com/tvoj-username/BookHaven.git](https://github.com/tvoj-username/BookHaven.git)
cd BookHaven

### 3. Database & Migrations Setup
Open the BookHavenWeb project and locate appsettings.json.

Update the DefaultConnection string with your SQL Server name:

"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=BookHaven;Trusted_Connection=True;TrustServerCertificate=True"
}

Open your terminal in the solution root and run the following command to create the database and tables:
dotnet ef database update --project BookHaven.DataAccess --startup-project BookHavenWeb

Launch the Application
Set BookHavenWeb as the Startup Project in Rider or Visual Studio.

Run the application (press F5 or use the Run button).

The system will be live at the local address provided in your terminal (usually https://localhost:7xxx).
