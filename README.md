# Language Center Management System

A comprehensive web application for managing a language learning center, built with ASP.NET Core 9.0, Entity Framework Core, and SQL Server.

## Overview

The Language Center Management System is a full-featured web application that provides role-based access for administrators, teachers, and students. It streamlines course management, enrollment processes, class scheduling, attendance tracking, and academic result monitoring.

## Features

### Authentication & Authorization
- ASP.NET Core Identity for user management
- Role-based access control (Admin, Teacher, Student)
- Custom registration flow with role selection
- Secure login with account lockout protection
- Account activation/deactivation functionality

### Admin Portal (`/Admin`)
- **Dashboard**: Statistics overview with user metrics and enrollment trends
- **User Management**: Complete CRUD operations for all users
- **Teacher Management**: Teacher profile management and expertise tracking
- **Student Management**: Student profile management and enrollment tracking
- **Course Management**: Create and manage language courses with pricing
- **Class Sessions**: Schedule and manage class sessions
- **Enrollment Management**: Monitor and approve student enrollments
- **Attendance Management**: Track and manage student attendance
- **Results Management**: Academic performance tracking and grading

### Teacher Portal (`/Teacher`)
- **Dashboard**: Overview of upcoming classes and quick actions
- **My Sessions**: View and manage assigned class sessions
- **Attendance Management**: Take attendance for class sessions
- **Profile Management**: Update teaching credentials and expertise
- Search and filter functionality for sessions
- Attendance statistics and reporting

### Student Portal (`/Student`)
- **Dashboard**: Personal overview with enrolled courses
- **Course Catalog**: Browse available courses with search/filter
- **Course Details**: Detailed course information and enrollment options
- **My Enrollments**: Track enrollment status and progress
- **Class Schedule**: View upcoming and completed class sessions
- **My Results**: Academic performance and grades
- Responsive design with intuitive navigation

## Technical Architecture

### Backend Technologies
- **Framework**: ASP.NET Core 9.0
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Architecture**: MVC with Razor Pages for admin areas
- **ORM**: Entity Framework Core with Code-First migrations

### Frontend Technologies
- **UI Framework**: Bootstrap 5.3.3
- **Icons**: Bootstrap Icons & Font Awesome
- **JavaScript**: Vanilla JS for interactive features
- **Responsive Design**: Mobile-first approach

### Database Schema
```
ApplicationUser (Identity)
├── StudentProfile (1:1)
├── TeacherProfile (1:1)
├── Enrollments (1:N)
├── ClassSessions (1:N as Teacher)
├── Attendances (1:N as Student)
└── Results (1:N as Student)

Course
├── ClassSessions (1:N)
├── Enrollments (1:N)
└── Results (1:N)

ClassSession
├── Course (N:1)
├── Teacher (N:1)
└── Attendances (1:N)

Enrollment
├── Student (N:1)
└── Course (N:1)

Attendance
├── Student (N:1)
└── ClassSession (N:1)

Result
├── Student (N:1)
└── Course (N:1)
```

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code
- Git

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd LanguageCenter
   ```

2. **Navigate to the project directory**
   ```bash
   cd LanguageCenter
   ```

3. **Update connection string**
   Edit `appsettings.json` and update the connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost,1433;Database=LanguageCenterDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Install dependencies**
   ```bash
   dotnet restore
   ```

5. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

6. **Run the application**
   ```bash
   dotnet run
   ```

7. **Access the application**
   - HTTP: `http://localhost:5255`
   - HTTPS: `https://localhost:7272`

## Database Setup

The application uses Entity Framework Core migrations for database setup. Key migrations include:

- `InitialCreate`: Basic Identity tables
- `MakeAvatarUrlNullable`: Avatar URL optimization
- `AddStudentTeacherProfiles`: User profile extensions
- `AddCourseEntity`: Course management
- `ConfigureTuitionFeePrecision`: Financial data precision
- `CreateAttendancesTable`: Attendance and session tracking
- `AddCreatedDateToApplicationUser`: User audit trail

### Sample Data
The application automatically seeds:
- Default admin user (configured in `DbInitializer`)
- User roles (Admin, Teacher, Student)

## 👥 User Roles & Permissions

### Admin
- Full system access
- User management (create, read, update, delete)
- Course and session management
- Enrollment approval and management
- System statistics and reporting

### Teacher
- View assigned class sessions
- Take attendance for classes
- View enrolled students
- Update personal profile
- Access to teaching-related features

### Student
- Browse and view course catalog
- Enroll in courses
- View enrollment status
- Check class schedules
- View academic results
- Update personal information

## 🔧 Configuration

### Application Settings
Key configuration options in `appsettings.json`:
- Database connection strings
- Logging configuration
- Kestrel server settings
- SSL certificate configuration

### Identity Configuration
- Email confirmation: Disabled (development)
- Account lockout: Configurable
- Password requirements: Standard ASP.NET Core defaults
- Two-factor authentication: Available

## API Endpoints

### Authentication Routes
- `GET /Account/SelectRole` - Role selection page
- `POST /Account/Register` - User registration
- `GET/POST /Identity/Account/Login` - User login
- `POST /Identity/Account/Logout` - User logout

### Admin Area Routes
- `/Admin/Dashboard` - Admin dashboard
- `/Admin/Users` - User management
- `/Admin/Teachers` - Teacher management
- `/Admin/Students` - Student management
- `/Admin/Courses` - Course management
- `/Admin/ClassSessions` - Session management
- `/Admin/Enrollments` - Enrollment management
- `/Admin/Attendance` - Attendance management

### Teacher Area Routes
- `/Teacher/Home` - Teacher dashboard
- `/Teacher/Sessions` - Session management
- `/Teacher/Attendance` - Attendance tracking
- `/Teacher/Profile` - Profile management

### Student Area Routes
- `/Student/Home` - Student dashboard
- `/Student/Courses` - Course catalog
- `/Student/Enrollments` - My enrollments
- `/Student/Schedule` - Class schedule
- `/Student/Results` - Academic results

## UI/UX Features

- **Responsive Design**: Mobile-first Bootstrap 5 implementation
- **Role-based Navigation**: Dynamic menus based on user roles
- **Status Indicators**: Color-coded badges for various statuses
- **Search & Filter**: Advanced filtering across all major entities
- **Pagination**: Efficient data loading for large datasets
- **Breadcrumb Navigation**: Clear navigation hierarchy
- **Success/Error Messages**: User-friendly feedback system
- **Loading States**: Smooth user experience with proper loading indicators

## Security Features

- **SQL Injection Prevention**: Entity Framework parameterized queries
- **XSS Protection**: Razor view engine automatic encoding
- **CSRF Protection**: Anti-forgery tokens on forms
- **Authentication Required**: Protected routes with [Authorize] attributes
- **Role-based Authorization**: Granular access control
- **Secure Password Storage**: ASP.NET Core Identity hashing
- **HTTPS Enforcement**: SSL/TLS encryption in production

## Business Logic

### Enrollment Process
1. Student browses course catalog
2. Student enrolls in course (status: "Pending")
3. Admin reviews and approves enrollment (status: "Confirmed")
4. Student attends classes and receives grades
5. Course completion (status: "Completed")

### Attendance Tracking
- Teachers take attendance for each class session
- Students can view their attendance history
- Attendance statistics for performance analysis

### Grade Management
- Automatic letter grade calculation (A-F)
- Numeric score tracking
- Progress monitoring and reporting

## Testing & Development

### Development Environment
- Entity Framework In-Memory database for testing
- Hot reload enabled for rapid development
- Detailed error pages in development mode
- Comprehensive logging configuration

### Testing Strategy
- Unit tests for business logic
- Integration tests for database operations
- UI tests for critical user workflows

## Performance Optimizations

- **Database Indexing**: Optimized queries with proper indexes
- **Lazy Loading**: Efficient data loading strategies
- **Caching**: Strategic caching for frequently accessed data
- **Pagination**: Limited data loading for better performance
- **Asset Optimization**: Minified CSS/JS in production

## Deployment

### Production Configuration
1. Update connection strings for production database
2. Configure SSL certificates
3. Set environment to "Production"
4. Enable error logging and monitoring
5. Configure backup strategies

### Environment Variables
```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=https://+:443;http://+:80
ConnectionStrings__DefaultConnection=<production-connection-string>
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

## Author

**Nhan Ho**
- Project Developer and Maintainer
- Language Center Management System

**Nguyen Nguyen**
- Project Lead
- Project Manager

## Support

For support and questions:
- Check the existing documentation
- Review the source code comments
- Create an issue for bug reports
- Submit feature requests through the issue tracker

---
