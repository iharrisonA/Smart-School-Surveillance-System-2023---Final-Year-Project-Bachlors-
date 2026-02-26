# 🏫 Smart School Surveillance System (SSSS)

**Final Year Project — BS Computer Science**  
Iqra University, Karachi | November 2023

**Group Members:**
- Muhammad Osama Fazal Siddiqui (54386)
- Naveed Akhter (54359)
- Harrison Arnold (54328)

**Supervisor:** Asif Ali Shahmiri  
**Coordinator:** Dr. Atiya Masood

---

## 📋 Project Overview

The Smart School Surveillance System (SSSS) is a comprehensive web-based application designed to revolutionize school administration. It provides centralized management for administrators, teachers, and students with real-time tracking of attendance, academic progress, communications, and fee management.

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | Python 3 + Flask |
| Database | SQLite (via Python sqlite3) |
| Frontend | HTML5, CSS3, JavaScript |
| Fonts | Space Grotesk, DM Sans (Google Fonts) |
| Icons | Font Awesome 6.4 |

---

## ✅ Features

### 👑 Admin Portal
- Dashboard with system statistics
- Add/Edit/Delete Students (with parent details)
- Add/Edit/Delete Teachers
- Manage Classes & Subjects
- Assign Subjects to Teachers
- Create & Broadcast Announcements
- Approve/Reject Student Applications
- Set Fee Structure per Class (with addons)
- Generate & Print Fee Vouchers (with discount)
- Add Additional Admin Users

### 👨‍🏫 Teacher Portal
- Dashboard with subject overview
- Take Attendance per Subject/Class
- Enter Marks (Mid Term / Final / Quiz etc.)
- Upload Lectures & Assignments
- Create Subject-specific Announcements
- Answer Student Q&A

### 🎓 Student Portal
- Personal Dashboard with attendance %
- View Attendance Records
- View Marks with Grade & Progress Bar
- Access Study Materials & Assignments
- Submit Applications (Leave, etc.)
- Ask Questions to Teachers (Q&A)

---

## 🚀 Setup & Run

### Prerequisites
- Python 3.8 or higher
- pip (Python package installer)

### Step 1: Clone / Download the Project
```bash
git clone <your-repo-url>
cd ssss
```

### Step 2: Install Dependencies
```bash
pip install flask
```

### Step 3: Run the Application
```bash
python app.py
```

### Step 4: Open in Browser
Navigate to: **http://localhost:5000**

The database (`ssss.db`) is created automatically on first run with demo data.

---

## 🔑 Demo Login Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@ssss.com | admin123 |
| Teacher | teacher@ssss.com | teacher123 |
| Student | student@ssss.com | student123 |

---

## 📁 Project Structure

```
ssss/
├── app.py                    # Main Flask application (all routes & DB logic)
├── requirements.txt          # Python dependencies
├── ssss.db                   # SQLite database (auto-created)
├── README.md                 # This file
└── templates/
    ├── base.html             # Shared layout (sidebar, topbar, CSS)
    ├── login.html            # Login page
    ├── admin/
    │   ├── dashboard.html    # Admin dashboard
    │   ├── students.html     # Student list
    │   ├── add_student.html  # Register student
    │   ├── edit_student.html # Edit student
    │   ├── teachers.html     # Teacher list
    │   ├── add_teacher.html  # Register teacher
    │   ├── edit_teacher.html # Edit teacher
    │   ├── classes.html      # Class management
    │   ├── subjects.html     # Subject management
    │   ├── assign_subject.html # Assign subject to teacher
    │   ├── announcements.html  # Announcements
    │   ├── applications.html   # Student applications
    │   ├── fees.html          # Fee structure
    │   ├── voucher.html       # Generate voucher
    │   ├── view_voucher.html  # Printable voucher
    │   └── add_user.html      # Add admin user
    ├── teacher/
    │   ├── dashboard.html
    │   ├── attendance.html
    │   ├── marks.html
    │   ├── lectures.html
    │   ├── announcements.html
    │   └── qa.html
    └── student/
        ├── dashboard.html
        ├── attendance.html
        ├── marks.html
        ├── materials.html
        ├── applications.html
        └── qa.html
```

---

## 🗄️ Database Schema

| Table | Description |
|-------|-------------|
| `users` | All system users (admin, teacher, student) |
| `students` | Student profiles and parent info |
| `teachers` | Teacher profiles |
| `classes` | School classes |
| `subjects` | Subjects per class |
| `teacher_subjects` | Teacher-subject-class assignments |
| `announcements` | School announcements |
| `attendance` | Daily attendance records |
| `marks` | Exam marks per student/subject |
| `lectures` | Uploaded lectures and assignments |
| `applications` | Student leave applications |
| `fees` | Fee structure per class |
| `vouchers` | Generated fee vouchers |
| `qa` | Student questions & teacher answers |

---

## 📊 Use Cases Implemented

1. ✅ Login (Admin, Teacher, Student)
2. ✅ Manage Class
3. ✅ Manage Student
4. ✅ Manage Teacher
5. ✅ Admin Announcement
6. ✅ Approve/Reject Application
7. ✅ Generate Voucher
8. ✅ Teacher Announcement
9. ✅ Take Attendance
10. ✅ Manage Lectures & Assignments
11. ✅ Assign Marks
12. ✅ Q & A
13. ✅ Request Application (Student)
14. ✅ Logout

---

## 🔒 Security

- Passwords stored as SHA-256 hashes
- Session-based authentication
- Role-based access control (Admin/Teacher/Student)
- Route-level authorization checks

---

## 🌐 Browser Support

Chrome, Firefox, Safari, Edge (all modern browsers)

---

*Built with ❤️ for Iqra University FYP — Smart School Surveillance System*
