# URL Shorten

URL Shorten is an ASP.NET Core MVC application that allows users to generate short URLs from any given link. It includes user authentication, QR code generation for each shortened URL, and a responsive design using Bootstrap and the AdminKit template. Non-authenticated users can create up to three short URLs stored in cookies, with the option to create more after logging in. Authenticated users can manage their URLs, update their profiles, and have any pre-login URLs automatically migrated from cookies to their account.

## Features

- **Short URL Generation**: Users can shorten any link by providing a custom keyword (4-15 characters). The application checks if the keyword is available before generating the short URL.
- **QR Code**: Each generated short URL comes with an associated QR code for easy sharing.
- **User Authentication**: Users can register, log in, and manage their profiles.
- **Cookie Management**: Non-authenticated users can create up to three short URLs stored in cookies. Upon login, these URLs are migrated to the user's account.
- **Responsive Design**: The application uses Bootstrap and the AdminKit template to ensure a responsive and user-friendly interface across devices.
- **User Profile Management**: Authenticated users can update their name and email.
- **Entity Framework Core**: For database management with SQL Server.
- **Repository Pattern and Unit of Work**: Ensures clean and maintainable code structure.
- **AJAX Integration**: For smooth user experience and dynamic content updates without full-page reloads.
- **SweetAlert2 Integration**: For modern and customizable alert dialogs.

## Project Structure

The project is organized using an n-tier architecture:

**ApplicationIdentity Layer**: ASP.NET Core MVC application layer exclusively for managing Identity-related tasks, including DbContext, migrations, and identity classes. (Located in `UrlShorten.ApplicationIdentity`)
- **Data Access Layer**: Manages database interactions using Entity Framework Core with a separate DbContext for application-related data, employing the repository pattern and unit of work. (Located in `UrlShorten.Application`)
- **Models Layer**: Contains the models, entities, and classes used throughout the application. (Located in `UrlShorten.Models`)
- **Web Layer**: ASP.NET Core MVC layer containing controllers, views, and static pages for the application. (Located in `UrlShorten.Web`)
  


## Technologies Used

- ![ASP.NET Core MVC 8](https://img.shields.io/badge/ASP.NET%20Core%20MVC-8.0-blue.svg)
- **Entity Framework Core**
- **SQL Server**
- **Bootstrap 5**
- **AdminKit (Free Bootstrap Template)**
- **SweetAlert2**
- **AJAX**
- **QR Code Generation**
- **Repository Pattern**
- **Unit of Work**
- **Identity Framework**
- **Cookies for Temporary Storage**

