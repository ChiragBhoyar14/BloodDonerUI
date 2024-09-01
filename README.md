# BloodDonerUI

## Overview

BloodDoner is an ASP.NET Core MVC web application that allows users to register as blood donors, log in, and view available donors. The application integrates with external APIs to manage and retrieve donor information. It utilizes JWT for authentication and session management.

## Features

- **User Authentication**: Login and register users with JWT token-based authentication.
- **Donor Registration**: Register new blood donors with details like state, blood group, and other personal information.
- **Data Integration**: Fetch and display states, blood groups, and cities from external APIs.
- **Session Management**: Maintain user sessions using ASP.NET Core session management.

## Technologies

- ASP.NET Core MVC
- C#
- JWT (JSON Web Tokens)
- HTTP Client
- Entity Framework Core (if applicable)
- HTML, CSS, JavaScript
- External APIs for data retrieval

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download) or later
- [Visual Studio](https://visualstudio.microsoft.com/) or any other C# IDE
- [Postman](https://www.postman.com/) for API testing (optional)

## Setup

### Clone the Repository

```bash
git clone https://github.com/ChiragBhoyar14/BloodDoner.git
cd BloodDoner
