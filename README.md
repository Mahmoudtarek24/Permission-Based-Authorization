# Permission-Based Authorization System

## 📋 Overview
ASP.NET Core Web API project that simulates the advanced permission management system found in ASP.NET Identity, with additional features for managing permissions at the individual user level.

## 🎯 Project Goals
- Complete simulation of core ASP.NET Identity tables
- Implementation of an advanced permission system that supports:
  - Role-based permissions
  - User-specific permissions
  - Ability to add or remove specific permissions for individual users

## 🗄️ Database Structure

### Core Tables (Identity Simulation):
- **Users** - Users table
- **Roles** - Roles table
- **UserRoles** - User-Role relationship table
- **Permissions** - Permissions table
- **RolePermissions** - Role-Permission relationship table
- **UserPermissions** - Additional/removed permissions at user level

### Additional Features:
- **System Roles**: Protected roles that cannot be deleted
- **System Permissions**: Core system permissions

## 🔧 Technologies Used
- **ASP.NET Core 8.0** - Web API Framework
- **Entity Framework Core** - ORM for database operations
- **JWT Authentication** - Authentication system
- **Custom Authorization Handlers** - Custom permission handlers

## 🚀 Key Features

### 1. User Management
- Create, update, delete users
- Enable/disable users
- Assign multiple roles to a single user

### 2. Role Management
- Create custom roles
- Link permissions to roles
- Protect system roles from deletion

### 3. Permission Management
- Hierarchical permission system (Module.Action)
- Ability to add new permissions
- Group permissions by module

### 4. Flexible Permissions
- **Role-based Permissions**: Users inherit permissions from their roles
- **Additional Permissions**: Grant extra permissions to specific users
- **Revoked Permissions**: Remove specific permissions from users even if they have them through roles

## 📚 API Documentation
The system provides comprehensive REST API endpoints organized into different controllers:
<img width="1672" height="810" alt="Screenshot 2025-07-24 103032" src="https://github.com/user-attachments/assets/84e88647-c025-46ba-b376-484785d5b380" />

<img width="1645" height="456" alt="Screenshot 2025-07-24 103043" src="https://github.com/user-attachments/assets/8ccdf9f7-51d8-4072-a090-87609e505f01" />
