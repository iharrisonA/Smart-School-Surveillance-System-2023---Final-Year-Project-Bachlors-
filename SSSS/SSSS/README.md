# 🏫 Smart School Surveillance System (SSSS)

A full-stack web-based school management system built with **ASP.NET Core MVC**, **C#**, and **SQL Server**. Implements three role-based portals — Admin, Teacher, and Student — to digitise and streamline day-to-day school operations.

---

## 📁 Project Structure

```
SSSS/
├── Controllers/
│   ├── AccountController.cs        # Login / Logout
│   ├── AdminController.cs          # Full admin portal
│   └── TeacherStudentController.cs # Teacher & Student portals
│
├── Models/
│   └── Models.cs                   # All domain models & ViewModels
│
├── Data/
│   └── AppDbContext.cs             # EF Core DbContext with seed data
│
├── Views/
│   ├── Account/    Login.cshtml
│   ├── Admin/      Dashboard, Students, Teachers, Classes,
│   │               Subjects, AssignSubject, Fees, Applications,
│   │               Announcements, ViewVoucher, AddUser
│   ├── Teacher/    Dashboard, Attendance, Marks, Lectures,
│   │               Announcements, QA
│   ├── Student/    Dashboard, Attendance, Marks, Materials,
│   │               Fees, Applications, QA
│   └── Shared/     _Layout.cshtml
│
├── Program.cs
├── appsettings.json
└── SSSS.csproj
```

---

## ⚙️ Setup & Run

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) or SQL Server LocalDB (included with Visual Studio)

### Steps

```bash
# 1. Clone the repository
git clone https://github.com/your-username/SSSS.git
cd SSSS/SSSS

# 2. Update connection string in appsettings.json
#    Default uses (localdb)\mssqllocaldb — works out of the box with Visual Studio

# 3. Apply database migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

# 4. Run the application
dotnet run
```

Open your browser at `https://localhost:5001`

---

## 🔐 Default Credentials

| Role    | Email              | Password   |
|---------|--------------------|------------|
| Admin   | admin@ssss.edu     | Admin@123  |

Use the Admin portal to create Teacher and Student accounts.

---

## 🚀 Features

### 👨‍💼 Admin Portal
- Dashboard with live stats (students, teachers, classes, pending items)
- Full CRUD for Students and Teachers
- Class and Subject management
- Assign subjects to teachers per class
- Generate and manage Fee Vouchers (with printable receipt)
- Review and approve/reject Leave Applications
- View all Announcements

### 👩‍🏫 Teacher Portal
- Dashboard with personal class/subject summary
- Mark attendance per subject per class per day
- Enter student marks by exam type (Quiz, Midterm, Final, Assignment)
- Upload lecture materials and files
- Post announcements to students
- Answer student questions (Q&A)

### 🎓 Student Portal
- Dashboard with attendance rate and pending fees
- View personal attendance records with subject breakdown
- View marks and percentage by subject and exam type
- Download lecture materials uploaded by teachers
- View and print fee vouchers
- Submit leave applications and track approval status
- Ask questions and receive teacher answers (Q&A)

---

## 🗄️ Database Schema (13 Tables)

| Table               | Description                              |
|---------------------|------------------------------------------|
| Users               | Base account (Admin/Teacher/Student)     |
| Teachers            | Teacher profiles                         |
| Students            | Student profiles                         |
| Classes             | School classes (e.g. 10-A)               |
| Subjects            | Academic subjects                        |
| SubjectAssignments  | Teacher → Subject → Class mapping        |
| Attendances         | Daily attendance records                 |
| Marks               | Academic marks per exam type             |
| FeeVouchers         | Monthly fee vouchers                     |
| Announcements       | Teacher announcements                    |
| LectureMaterials    | Uploaded study files                     |
| LeaveApplications   | Student leave requests                   |
| QAQuestions         | Student questions                        |
| QAAnswers           | Teacher answers                          |

---

## 🛠 Tech Stack

- **C# / ASP.NET Core 8 MVC** — Backend framework
- **Entity Framework Core 8** — ORM with Code First migrations
- **SQL Server / LocalDB** — Relational database
- **BCrypt.Net** — Password hashing
- **Bootstrap 5** — Responsive frontend UI
- **Bootstrap Icons** — Icon set

---

## 📄 Final Year Project

This system was developed as a Final Year Project (FYP) to replace paper-based school administration with a digital, role-based web application. The system covers all 14 use cases defined in the project proposal including student/teacher management, attendance, marks, fee management, announcements, lecture materials, leave applications, and Q&A.
