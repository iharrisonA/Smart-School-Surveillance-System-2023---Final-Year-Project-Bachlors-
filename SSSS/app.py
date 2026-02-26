from flask import Flask, render_template, request, redirect, url_for, session, flash, jsonify
import sqlite3
import hashlib
import os
from datetime import datetime, date

app = Flask(__name__)
app.secret_key = 'ssss_secret_key_2023'
DATABASE = 'ssss.db'

def get_db():
    conn = sqlite3.connect(DATABASE)
    conn.row_factory = sqlite3.Row
    return conn

def hash_password(password):
    return hashlib.sha256(password.encode()).hexdigest()

def init_db():
    conn = get_db()
    c = conn.cursor()

    c.execute('''CREATE TABLE IF NOT EXISTS users (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        name TEXT NOT NULL,
        email TEXT UNIQUE NOT NULL,
        password TEXT NOT NULL,
        role TEXT NOT NULL,
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    )''')

    c.execute('''CREATE TABLE IF NOT EXISTS classes (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        class_name TEXT NOT NULL UNIQUE
    )''')

    c.execute('''CREATE TABLE IF NOT EXISTS subjects (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        subject_name TEXT NOT NULL,
        class_id INTEGER,
        FOREIGN KEY(class_id) REFERENCES classes(id)
    )''')

    c.execute('''CREATE TABLE IF NOT EXISTS students (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        user_id INTEGER,
        name TEXT NOT NULL,
        dob TEXT,
        gender TEXT,
        address TEXT,
        parent_name TEXT,
        parent_cnic TEXT,
        phone TEXT,
        email TEXT UNIQUE,
        password TEXT,
        class_id INTEGER,
        roll_number TEXT UNIQUE,
        FOREIGN KEY(class_id) REFERENCES classes(id),
        FOREIGN KEY(user_id) REFERENCES users(id)
    )''')

    c.execute('''CREATE TABLE IF NOT EXISTS teachers (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        user_id INTEGER,
        name TEXT NOT NULL,
        dob TEXT,
        email TEXT UNIQUE,
        password TEXT,
        gender TEXT,
        phone TEXT,
        address TEXT,
        cnic TEXT,
        FOREIGN KEY(user_id) REFERENCES users(id)
    )''')

    c.execute('''CREATE TABLE IF NOT EXISTS teacher_subjects (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        teacher_id INTEGER,
        subject_id INTEGER,
        class_id INTEGER,
        FOREIGN KEY(teacher_id) REFERENCES teachers(id),
        FOREIGN KEY(subject_id) REFERENCES subjects(id),
        FOREIGN KEY(class_id) REFERENCES classes(id)
    )''')

    c.execute('''CREATE TABLE IF NOT EXISTS announcements (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT NOT NULL,
        details TEXT,
        start_date TEXT,
        end_date TEXT,
        assigned_to TEXT DEFAULT 'all',
        created_by INTEGER,
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    )''')

    c.execute('''CREATE TABLE IF NOT EXISTS attendance (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        student_id INTEGER,
        subject_id INTEGER,
        class_id INTEGER,
        date TEXT,
        status TEXT,
        FOREIGN KEY(student_id) REFERENCES students(id)
    )''')

    c.execute('''CREATE TABLE IF NOT EXISTS marks (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        student_id INTEGER,
        subject_id INTEGER,
        class_id INTEGER,
        marks_obtained INTEGER,
        total_marks INTEGER,
        exam_type TEXT,
        FOREIGN KEY(student_id) REFERENCES students(id)
    )''')

    c.execute('''CREATE TABLE IF NOT EXISTS lectures (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        title TEXT NOT NULL,
        description TEXT,
        file_name TEXT,
        subject_id INTEGER,
        class_id INTEGER,
        teacher_id INTEGER,
        type TEXT DEFAULT 'lecture',
        due_date TEXT,
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY(subject_id) REFERENCES subjects(id)
    )''')

    c.execute('''CREATE TABLE IF NOT EXISTS applications (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        student_id INTEGER,
        subject TEXT,
        details TEXT,
        send_to TEXT DEFAULT 'Admin',
        status TEXT DEFAULT 'Pending',
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY(student_id) REFERENCES students(id)
    )''')

    c.execute('''CREATE TABLE IF NOT EXISTS fees (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        class_id INTEGER UNIQUE,
        amount REAL,
        transport REAL DEFAULT 0,
        sports REAL DEFAULT 0,
        FOREIGN KEY(class_id) REFERENCES classes(id)
    )''')

    c.execute('''CREATE TABLE IF NOT EXISTS vouchers (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        student_id INTEGER,
        roll_number TEXT,
        amount REAL,
        discount REAL DEFAULT 0,
        total_payable REAL,
        till_date TEXT,
        generated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY(student_id) REFERENCES students(id)
    )''')

    c.execute('''CREATE TABLE IF NOT EXISTS qa (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        student_id INTEGER,
        teacher_id INTEGER,
        subject_id INTEGER,
        question TEXT,
        answer TEXT,
        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY(student_id) REFERENCES students(id)
    )''')

    # Insert default admin
    try:
        c.execute("INSERT INTO users (name, email, password, role) VALUES (?, ?, ?, ?)",
                  ('Administrator', 'admin@ssss.com', hash_password('admin123'), 'admin'))
    except:
        pass

    # Insert demo classes
    classes = ['One','Two','Three','Four','Five','Six','Seven','Eight','Nine','Matric','Prep-1','Prep-2']
    for cls in classes:
        try:
            c.execute("INSERT INTO classes (class_name) VALUES (?)", (cls,))
        except:
            pass

    # Insert demo subjects
    subjects = ['Mathematics','English','Science','Urdu','Islamiat','Computer','Physics','Chemistry','Biology']
    for subj in subjects:
        try:
            c.execute("INSERT INTO subjects (subject_name, class_id) VALUES (?, 1)", (subj,))
        except:
            pass

    # Insert demo teacher
    try:
        c.execute("INSERT INTO users (name, email, password, role) VALUES (?, ?, ?, ?)",
                  ('Ali Hassan', 'teacher@ssss.com', hash_password('teacher123'), 'teacher'))
        uid = c.lastrowid
        c.execute("INSERT INTO teachers (user_id, name, dob, email, password, gender, phone, address, cnic) VALUES (?,?,?,?,?,?,?,?,?)",
                  (uid, 'Ali Hassan', '1985-06-15', 'teacher@ssss.com', hash_password('teacher123'), 'Male', '0300-1234567', 'Karachi', '42101-1234567-1'))
    except:
        pass

    # Insert demo student
    try:
        c.execute("INSERT INTO users (name, email, password, role) VALUES (?, ?, ?, ?)",
                  ('Ahmed Khan', 'student@ssss.com', hash_password('student123'), 'student'))
        uid = c.lastrowid
        c.execute("INSERT INTO students (user_id, name, dob, gender, address, parent_name, parent_cnic, phone, email, password, class_id, roll_number) VALUES (?,?,?,?,?,?,?,?,?,?,?,?)",
                  (uid, 'Ahmed Khan', '2005-03-20', 'Male', 'Karachi', 'Khan Sr.', '42101-9876543-1', '0321-9876543', 'student@ssss.com', hash_password('student123'), 1, '1001'))
    except:
        pass

    conn.commit()
    conn.close()

# ─── AUTH ────────────────────────────────────────────────────────────────────

@app.route('/')
def index():
    return redirect(url_for('login'))

@app.route('/login', methods=['GET', 'POST'])
def login():
    if request.method == 'POST':
        email = request.form['email']
        password = hash_password(request.form['password'])
        conn = get_db()
        user = conn.execute("SELECT * FROM users WHERE email=? AND password=?", (email, password)).fetchone()
        conn.close()
        if user:
            session['user_id'] = user['id']
            session['user_name'] = user['name']
            session['user_role'] = user['role']
            session['user_email'] = user['email']
            if user['role'] == 'admin':
                return redirect(url_for('admin_dashboard'))
            elif user['role'] == 'teacher':
                return redirect(url_for('teacher_dashboard'))
            else:
                return redirect(url_for('student_dashboard'))
        else:
            flash('Invalid email or password.', 'danger')
    return render_template('login.html')

@app.route('/logout')
def logout():
    session.clear()
    return redirect(url_for('login'))

def login_required(role=None):
    def decorator(f):
        from functools import wraps
        @wraps(f)
        def decorated(*args, **kwargs):
            if 'user_id' not in session:
                return redirect(url_for('login'))
            if role and session.get('user_role') != role:
                flash('Access denied.', 'danger')
                return redirect(url_for('login'))
            return f(*args, **kwargs)
        return decorated
    return decorator

# ─── ADMIN ───────────────────────────────────────────────────────────────────

@app.route('/admin')
@login_required('admin')
def admin_dashboard():
    conn = get_db()
    stats = {
        'teachers': conn.execute("SELECT COUNT(*) as c FROM teachers").fetchone()['c'],
        'students': conn.execute("SELECT COUNT(*) as c FROM students").fetchone()['c'],
        'classes': conn.execute("SELECT COUNT(*) as c FROM classes").fetchone()['c'],
        'subjects': conn.execute("SELECT COUNT(*) as c FROM subjects").fetchone()['c'],
        'announcements': conn.execute("SELECT COUNT(*) as c FROM announcements").fetchone()['c'],
        'applications': conn.execute("SELECT COUNT(*) as c FROM applications").fetchone()['c'],
        'pending_apps': conn.execute("SELECT COUNT(*) as c FROM applications WHERE status='Pending'").fetchone()['c'],
    }
    announcements = conn.execute("SELECT * FROM announcements ORDER BY created_at DESC LIMIT 5").fetchall()
    conn.close()
    return render_template('admin/dashboard.html', stats=stats, announcements=announcements)

# Students
@app.route('/admin/students')
@login_required('admin')
def admin_students():
    conn = get_db()
    students = conn.execute("""
        SELECT s.*, c.class_name FROM students s
        LEFT JOIN classes c ON s.class_id = c.id
        ORDER BY s.id DESC
    """).fetchall()
    conn.close()
    return render_template('admin/students.html', students=students)

@app.route('/admin/students/add', methods=['GET', 'POST'])
@login_required('admin')
def admin_add_student():
    conn = get_db()
    classes = conn.execute("SELECT * FROM classes").fetchall()
    if request.method == 'POST':
        f = request.form
        try:
            conn.execute("INSERT INTO users (name, email, password, role) VALUES (?,?,?,?)",
                         (f['name'], f['email'], hash_password(f['password']), 'student'))
            uid = conn.execute("SELECT last_insert_rowid()").fetchone()[0]
            conn.execute("""INSERT INTO students (user_id, name, dob, gender, address, parent_name, parent_cnic, phone, email, password, class_id, roll_number)
                            VALUES (?,?,?,?,?,?,?,?,?,?,?,?)""",
                         (uid, f['name'], f['dob'], f['gender'], f['address'],
                          f['parent_name'], f['parent_cnic'], f['phone'], f['email'],
                          hash_password(f['password']), f['class_id'],
                          f'S{conn.execute("SELECT COUNT(*) as c FROM students").fetchone()["c"]+1000}'))
            conn.commit()
            flash('Student registered successfully!', 'success')
            return redirect(url_for('admin_students'))
        except Exception as e:
            flash(f'Error: {str(e)}', 'danger')
    conn.close()
    return render_template('admin/add_student.html', classes=classes)

@app.route('/admin/students/edit/<int:sid>', methods=['GET', 'POST'])
@login_required('admin')
def admin_edit_student(sid):
    conn = get_db()
    student = conn.execute("SELECT * FROM students WHERE id=?", (sid,)).fetchone()
    classes = conn.execute("SELECT * FROM classes").fetchall()
    if request.method == 'POST':
        f = request.form
        conn.execute("""UPDATE students SET name=?, dob=?, gender=?, address=?, parent_name=?,
                        parent_cnic=?, phone=?, class_id=? WHERE id=?""",
                     (f['name'], f['dob'], f['gender'], f['address'], f['parent_name'],
                      f['parent_cnic'], f['phone'], f['class_id'], sid))
        conn.commit()
        flash('Student updated!', 'success')
        return redirect(url_for('admin_students'))
    conn.close()
    return render_template('admin/edit_student.html', student=student, classes=classes)

@app.route('/admin/students/delete/<int:sid>')
@login_required('admin')
def admin_delete_student(sid):
    conn = get_db()
    conn.execute("DELETE FROM students WHERE id=?", (sid,))
    conn.commit()
    conn.close()
    flash('Student deleted.', 'success')
    return redirect(url_for('admin_students'))

# Teachers
@app.route('/admin/teachers')
@login_required('admin')
def admin_teachers():
    conn = get_db()
    teachers = conn.execute("SELECT * FROM teachers ORDER BY id DESC").fetchall()
    conn.close()
    return render_template('admin/teachers.html', teachers=teachers)

@app.route('/admin/teachers/add', methods=['GET', 'POST'])
@login_required('admin')
def admin_add_teacher():
    if request.method == 'POST':
        f = request.form
        conn = get_db()
        try:
            conn.execute("INSERT INTO users (name, email, password, role) VALUES (?,?,?,?)",
                         (f['name'], f['email'], hash_password(f['password']), 'teacher'))
            uid = conn.execute("SELECT last_insert_rowid()").fetchone()[0]
            conn.execute("""INSERT INTO teachers (user_id, name, dob, email, password, gender, phone, address, cnic)
                            VALUES (?,?,?,?,?,?,?,?,?)""",
                         (uid, f['name'], f['dob'], f['email'], hash_password(f['password']),
                          f['gender'], f['phone'], f['address'], f['cnic']))
            conn.commit()
            flash('Teacher added!', 'success')
            return redirect(url_for('admin_teachers'))
        except Exception as e:
            flash(f'Error: {str(e)}', 'danger')
        conn.close()
    return render_template('admin/add_teacher.html')

@app.route('/admin/teachers/edit/<int:tid>', methods=['GET', 'POST'])
@login_required('admin')
def admin_edit_teacher(tid):
    conn = get_db()
    teacher = conn.execute("SELECT * FROM teachers WHERE id=?", (tid,)).fetchone()
    if request.method == 'POST':
        f = request.form
        conn.execute("""UPDATE teachers SET name=?, dob=?, gender=?, phone=?, address=?, cnic=? WHERE id=?""",
                     (f['name'], f['dob'], f['gender'], f['phone'], f['address'], f['cnic'], tid))
        conn.commit()
        flash('Teacher updated!', 'success')
        return redirect(url_for('admin_teachers'))
    conn.close()
    return render_template('admin/edit_teacher.html', teacher=teacher)

@app.route('/admin/teachers/delete/<int:tid>')
@login_required('admin')
def admin_delete_teacher(tid):
    conn = get_db()
    conn.execute("DELETE FROM teachers WHERE id=?", (tid,))
    conn.commit()
    conn.close()
    flash('Teacher deleted.', 'success')
    return redirect(url_for('admin_teachers'))

# Classes
@app.route('/admin/classes')
@login_required('admin')
def admin_classes():
    conn = get_db()
    classes = conn.execute("SELECT * FROM classes ORDER BY id").fetchall()
    conn.close()
    return render_template('admin/classes.html', classes=classes)

@app.route('/admin/classes/add', methods=['POST'])
@login_required('admin')
def admin_add_class():
    conn = get_db()
    try:
        conn.execute("INSERT INTO classes (class_name) VALUES (?)", (request.form['class_name'],))
        conn.commit()
        flash('Class added!', 'success')
    except:
        flash('Class already exists.', 'danger')
    conn.close()
    return redirect(url_for('admin_classes'))

@app.route('/admin/classes/delete/<int:cid>')
@login_required('admin')
def admin_delete_class(cid):
    conn = get_db()
    conn.execute("DELETE FROM classes WHERE id=?", (cid,))
    conn.commit()
    conn.close()
    flash('Class deleted.', 'success')
    return redirect(url_for('admin_classes'))

# Subjects
@app.route('/admin/subjects')
@login_required('admin')
def admin_subjects():
    conn = get_db()
    subjects = conn.execute("""
        SELECT s.*, c.class_name FROM subjects s
        LEFT JOIN classes c ON s.class_id = c.id
        ORDER BY s.id
    """).fetchall()
    classes = conn.execute("SELECT * FROM classes").fetchall()
    conn.close()
    return render_template('admin/subjects.html', subjects=subjects, classes=classes)

@app.route('/admin/subjects/add', methods=['POST'])
@login_required('admin')
def admin_add_subject():
    conn = get_db()
    conn.execute("INSERT INTO subjects (subject_name, class_id) VALUES (?,?)",
                 (request.form['subject_name'], request.form['class_id']))
    conn.commit()
    conn.close()
    flash('Subject added!', 'success')
    return redirect(url_for('admin_subjects'))

@app.route('/admin/subjects/delete/<int:sid>')
@login_required('admin')
def admin_delete_subject(sid):
    conn = get_db()
    conn.execute("DELETE FROM subjects WHERE id=?", (sid,))
    conn.commit()
    conn.close()
    flash('Subject deleted.', 'success')
    return redirect(url_for('admin_subjects'))

# Assign Subject to Teacher
@app.route('/admin/assign-subject', methods=['GET', 'POST'])
@login_required('admin')
def admin_assign_subject():
    conn = get_db()
    teachers = conn.execute("SELECT * FROM teachers").fetchall()
    classes = conn.execute("SELECT * FROM classes").fetchall()
    subjects = conn.execute("SELECT s.*, c.class_name FROM subjects s LEFT JOIN classes c ON s.class_id=c.id").fetchall()
    assignments = conn.execute("""
        SELECT ts.*, t.name as teacher_name, s.subject_name, c.class_name
        FROM teacher_subjects ts
        JOIN teachers t ON ts.teacher_id = t.id
        JOIN subjects s ON ts.subject_id = s.id
        JOIN classes c ON ts.class_id = c.id
    """).fetchall()
    if request.method == 'POST':
        conn.execute("INSERT INTO teacher_subjects (teacher_id, subject_id, class_id) VALUES (?,?,?)",
                     (request.form['teacher_id'], request.form['subject_id'], request.form['class_id']))
        conn.commit()
        flash('Subject assigned!', 'success')
        return redirect(url_for('admin_assign_subject'))
    conn.close()
    return render_template('admin/assign_subject.html', teachers=teachers, classes=classes,
                           subjects=subjects, assignments=assignments)

# Announcements
@app.route('/admin/announcements', methods=['GET', 'POST'])
@login_required('admin')
def admin_announcements():
    conn = get_db()
    if request.method == 'POST':
        f = request.form
        conn.execute("INSERT INTO announcements (title, details, start_date, end_date, assigned_to, created_by) VALUES (?,?,?,?,?,?)",
                     (f['title'], f['details'], f['start_date'], f['end_date'], f.get('assigned_to', 'all'), session['user_id']))
        conn.commit()
        flash('Announcement created!', 'success')
    today = date.today().isoformat()
    announcements = conn.execute("SELECT * FROM announcements ORDER BY created_at DESC").fetchall()
    conn.close()
    return render_template('admin/announcements.html', announcements=announcements, today=today)

@app.route('/admin/announcements/delete/<int:aid>')
@login_required('admin')
def admin_delete_announcement(aid):
    conn = get_db()
    conn.execute("DELETE FROM announcements WHERE id=?", (aid,))
    conn.commit()
    conn.close()
    flash('Announcement deleted.', 'success')
    return redirect(url_for('admin_announcements'))

# Applications
@app.route('/admin/applications')
@login_required('admin')
def admin_applications():
    conn = get_db()
    apps = conn.execute("""
        SELECT a.*, s.name as student_name, s.roll_number
        FROM applications a
        JOIN students s ON a.student_id = s.id
        ORDER BY a.created_at DESC
    """).fetchall()
    conn.close()
    return render_template('admin/applications.html', applications=apps)

@app.route('/admin/applications/action/<int:aid>/<action>')
@login_required('admin')
def admin_application_action(aid, action):
    conn = get_db()
    status = 'Approved' if action == 'approve' else 'Rejected'
    conn.execute("UPDATE applications SET status=? WHERE id=?", (status, aid))
    conn.commit()
    conn.close()
    flash(f'Application {status}!', 'success')
    return redirect(url_for('admin_applications'))

# Fees
@app.route('/admin/fees', methods=['GET', 'POST'])
@login_required('admin')
def admin_fees():
    conn = get_db()
    classes = conn.execute("SELECT * FROM classes").fetchall()
    fees = conn.execute("SELECT f.*, c.class_name FROM fees f JOIN classes c ON f.class_id=c.id").fetchall()
    if request.method == 'POST':
        f = request.form
        try:
            conn.execute("INSERT INTO fees (class_id, amount, transport, sports) VALUES (?,?,?,?)",
                         (f['class_id'], float(f['amount']), float(f.get('transport', 0)), float(f.get('sports', 0))))
            conn.commit()
            flash('Fee structure added!', 'success')
        except Exception as e:
            flash(f'Error: Fee may already exist for this class.', 'danger')
    conn.close()
    return render_template('admin/fees.html', classes=classes, fees=fees)

@app.route('/admin/fees/delete/<int:fid>')
@login_required('admin')
def admin_delete_fee(fid):
    conn = get_db()
    conn.execute("DELETE FROM fees WHERE id=?", (fid,))
    conn.commit()
    conn.close()
    flash('Fee deleted.', 'success')
    return redirect(url_for('admin_fees'))

# Voucher
@app.route('/admin/voucher', methods=['GET', 'POST'])
@login_required('admin')
def admin_voucher():
    conn = get_db()
    student = None
    fee = None
    if request.method == 'POST':
        if 'search' in request.form:
            roll = request.form['roll_number']
            student = conn.execute("SELECT s.*, c.class_name FROM students s LEFT JOIN classes c ON s.class_id=c.id WHERE s.roll_number=?", (roll,)).fetchone()
            if student:
                fee = conn.execute("SELECT * FROM fees WHERE class_id=?", (student['class_id'],)).fetchone()
            else:
                flash('Student not found.', 'danger')
        elif 'generate' in request.form:
            f = request.form
            discount = float(f.get('discount', 0))
            base = float(f['base_amount'])
            transport = float(f.get('transport', 0))
            sports = float(f.get('sports', 0))
            total = base + transport + sports - discount
            conn.execute("INSERT INTO vouchers (student_id, roll_number, amount, discount, total_payable, till_date) VALUES (?,?,?,?,?,?)",
                         (f['student_id'], f['roll_number'], base, discount, total, f['till_date']))
            conn.commit()
            vid = conn.execute("SELECT last_insert_rowid()").fetchone()[0]
            conn.close()
            return redirect(url_for('view_voucher', vid=vid))
    conn.close()
    return render_template('admin/voucher.html', student=student, fee=fee)

@app.route('/admin/voucher/view/<int:vid>')
@login_required('admin')
def view_voucher(vid):
    conn = get_db()
    voucher = conn.execute("""
        SELECT v.*, s.name as student_name, s.parent_name, c.class_name
        FROM vouchers v
        JOIN students s ON v.student_id=s.id
        JOIN classes c ON s.class_id=c.id
        WHERE v.id=?
    """, (vid,)).fetchone()
    conn.close()
    return render_template('admin/view_voucher.html', voucher=voucher)

@app.route('/admin/add-user', methods=['GET', 'POST'])
@login_required('admin')
def admin_add_user():
    if request.method == 'POST':
        f = request.form
        conn = get_db()
        try:
            conn.execute("INSERT INTO users (name, email, password, role) VALUES (?,?,?,?)",
                         (f['name'], f['email'], hash_password(f['password']), 'admin'))
            conn.commit()
            flash('Admin user added!', 'success')
        except:
            flash('Email already exists.', 'danger')
        conn.close()
        return redirect(url_for('admin_dashboard'))
    return render_template('admin/add_user.html')

# ─── TEACHER ─────────────────────────────────────────────────────────────────

def get_teacher_id():
    conn = get_db()
    t = conn.execute("SELECT id FROM teachers WHERE email=?", (session.get('user_email'),)).fetchone()
    conn.close()
    return t['id'] if t else None

@app.route('/teacher')
@login_required('teacher')
def teacher_dashboard():
    tid = get_teacher_id()
    conn = get_db()
    assignments = conn.execute("""
        SELECT ts.*, s.subject_name, c.class_name
        FROM teacher_subjects ts
        JOIN subjects s ON ts.subject_id=s.id
        JOIN classes c ON ts.class_id=c.id
        WHERE ts.teacher_id=?
    """, (tid,)).fetchall()
    today = date.today().isoformat()
    announcements = conn.execute(
        "SELECT * FROM announcements WHERE (end_date >= ? OR end_date IS NULL) ORDER BY created_at DESC LIMIT 5", (today,)
    ).fetchall()
    stats = {
        'subjects': len(assignments),
        'lectures': conn.execute("SELECT COUNT(*) as c FROM lectures WHERE teacher_id=?", (tid,)).fetchone()['c'],
        'qa': conn.execute("SELECT COUNT(*) as c FROM qa WHERE teacher_id=? AND answer IS NULL", (tid,)).fetchone()['c'],
    }
    conn.close()
    return render_template('teacher/dashboard.html', assignments=assignments, announcements=announcements, stats=stats)

@app.route('/teacher/attendance', methods=['GET', 'POST'])
@login_required('teacher')
def teacher_attendance():
    tid = get_teacher_id()
    conn = get_db()
    assignments = conn.execute("""
        SELECT ts.*, s.subject_name, c.class_name, ts.class_id as cid, ts.subject_id as sid
        FROM teacher_subjects ts
        JOIN subjects s ON ts.subject_id=s.id
        JOIN classes c ON ts.class_id=c.id
        WHERE ts.teacher_id=?
    """, (tid,)).fetchall()
    students = []
    selected = None
    if request.method == 'POST' and 'fetch_students' in request.form:
        selected = {'class_id': request.form['class_id'], 'subject_id': request.form['subject_id']}
        students = conn.execute("SELECT * FROM students WHERE class_id=?", (selected['class_id'],)).fetchall()
    elif request.method == 'POST' and 'save_attendance' in request.form:
        f = request.form
        att_date = f['att_date']
        class_id = f['class_id']
        subject_id = f['subject_id']
        student_ids = f.getlist('student_ids')
        present_ids = f.getlist('present')
        for sid in student_ids:
            status = 'Present' if sid in present_ids else 'Absent'
            existing = conn.execute("SELECT id FROM attendance WHERE student_id=? AND subject_id=? AND date=?",
                                    (sid, subject_id, att_date)).fetchone()
            if existing:
                conn.execute("UPDATE attendance SET status=? WHERE id=?", (status, existing['id']))
            else:
                conn.execute("INSERT INTO attendance (student_id, subject_id, class_id, date, status) VALUES (?,?,?,?,?)",
                             (sid, subject_id, class_id, att_date, status))
        conn.commit()
        flash('Attendance saved!', 'success')
        return redirect(url_for('teacher_attendance'))
    conn.close()
    return render_template('teacher/attendance.html', assignments=assignments, students=students, selected=selected, today=date.today().isoformat())

@app.route('/teacher/marks', methods=['GET', 'POST'])
@login_required('teacher')
def teacher_marks():
    tid = get_teacher_id()
    conn = get_db()
    assignments = conn.execute("""
        SELECT ts.*, s.subject_name, c.class_name
        FROM teacher_subjects ts JOIN subjects s ON ts.subject_id=s.id
        JOIN classes c ON ts.class_id=c.id WHERE ts.teacher_id=?
    """, (tid,)).fetchall()
    students = []
    selected = None
    if request.method == 'POST' and 'fetch_students' in request.form:
        selected = {'class_id': request.form['class_id'], 'subject_id': request.form['subject_id'], 'exam_type': request.form['exam_type']}
        students = conn.execute("SELECT * FROM students WHERE class_id=?", (selected['class_id'],)).fetchall()
    elif request.method == 'POST' and 'save_marks' in request.form:
        f = request.form
        class_id = f['class_id']
        subject_id = f['subject_id']
        exam_type = f['exam_type']
        total = f['total_marks']
        student_ids = f.getlist('student_ids')
        for sid in student_ids:
            mo = f.get(f'marks_{sid}', 0)
            existing = conn.execute("SELECT id FROM marks WHERE student_id=? AND subject_id=? AND exam_type=?",
                                    (sid, subject_id, exam_type)).fetchone()
            if existing:
                conn.execute("UPDATE marks SET marks_obtained=?, total_marks=? WHERE id=?", (mo, total, existing['id']))
            else:
                conn.execute("INSERT INTO marks (student_id, subject_id, class_id, marks_obtained, total_marks, exam_type) VALUES (?,?,?,?,?,?)",
                             (sid, subject_id, class_id, mo, total, exam_type))
        conn.commit()
        flash('Marks saved!', 'success')
        return redirect(url_for('teacher_marks'))
    conn.close()
    return render_template('teacher/marks.html', assignments=assignments, students=students, selected=selected)

@app.route('/teacher/lectures', methods=['GET', 'POST'])
@login_required('teacher')
def teacher_lectures():
    tid = get_teacher_id()
    conn = get_db()
    assignments = conn.execute("""
        SELECT ts.*, s.subject_name, c.class_name
        FROM teacher_subjects ts JOIN subjects s ON ts.subject_id=s.id
        JOIN classes c ON ts.class_id=c.id WHERE ts.teacher_id=?
    """, (tid,)).fetchall()
    if request.method == 'POST':
        f = request.form
        conn.execute("""INSERT INTO lectures (title, description, subject_id, class_id, teacher_id, type, due_date)
                        VALUES (?,?,?,?,?,?,?)""",
                     (f['title'], f['description'], f['subject_id'], f['class_id'], tid, f['type'], f.get('due_date', '')))
        conn.commit()
        flash('Material uploaded!', 'success')
        return redirect(url_for('teacher_lectures'))
    lectures = conn.execute("""
        SELECT l.*, s.subject_name, c.class_name FROM lectures l
        JOIN subjects s ON l.subject_id=s.id JOIN classes c ON l.class_id=c.id
        WHERE l.teacher_id=? ORDER BY l.created_at DESC
    """, (tid,)).fetchall()
    conn.close()
    return render_template('teacher/lectures.html', assignments=assignments, lectures=lectures)

@app.route('/teacher/announcements', methods=['GET', 'POST'])
@login_required('teacher')
def teacher_announcements():
    tid = get_teacher_id()
    conn = get_db()
    if request.method == 'POST':
        f = request.form
        conn.execute("INSERT INTO announcements (title, details, start_date, end_date, assigned_to, created_by) VALUES (?,?,?,?,?,?)",
                     (f['title'], f['details'], f['start_date'], f['end_date'], 'students', session['user_id']))
        conn.commit()
        flash('Announcement posted!', 'success')
    announcements = conn.execute(
        "SELECT * FROM announcements WHERE created_by=? ORDER BY created_at DESC", (session['user_id'],)
    ).fetchall()
    today = date.today().isoformat()
    conn.close()
    return render_template('teacher/announcements.html', announcements=announcements, today=today)

@app.route('/teacher/qa', methods=['GET', 'POST'])
@login_required('teacher')
def teacher_qa():
    tid = get_teacher_id()
    conn = get_db()
    if request.method == 'POST':
        qid = request.form['qa_id']
        answer = request.form['answer']
        conn.execute("UPDATE qa SET answer=? WHERE id=?", (answer, qid))
        conn.commit()
        flash('Answer submitted!', 'success')
    questions = conn.execute("""
        SELECT q.*, s.name as student_name, sub.subject_name
        FROM qa q JOIN students s ON q.student_id=s.id
        LEFT JOIN subjects sub ON q.subject_id=sub.id
        WHERE q.teacher_id=? ORDER BY q.created_at DESC
    """, (tid,)).fetchall()
    conn.close()
    return render_template('teacher/qa.html', questions=questions)

# ─── STUDENT ──────────────────────────────────────────────────────────────────

def get_student_id():
    conn = get_db()
    s = conn.execute("SELECT id, class_id FROM students WHERE email=?", (session.get('user_email'),)).fetchone()
    conn.close()
    return (s['id'], s['class_id']) if s else (None, None)

@app.route('/student')
@login_required('student')
def student_dashboard():
    sid, cid = get_student_id()
    conn = get_db()
    today = date.today().isoformat()
    announcements = conn.execute(
        "SELECT * FROM announcements WHERE (end_date >= ? OR end_date IS NULL) ORDER BY created_at DESC LIMIT 5", (today,)
    ).fetchall()
    student = conn.execute("SELECT s.*, c.class_name FROM students s LEFT JOIN classes c ON s.class_id=c.id WHERE s.id=?", (sid,)).fetchone()
    att_total = conn.execute("SELECT COUNT(*) as c FROM attendance WHERE student_id=?", (sid,)).fetchone()['c']
    att_present = conn.execute("SELECT COUNT(*) as c FROM attendance WHERE student_id=? AND status='Present'", (sid,)).fetchone()['c']
    att_pct = round((att_present / att_total * 100) if att_total > 0 else 0)
    pending_apps = conn.execute("SELECT COUNT(*) as c FROM applications WHERE student_id=? AND status='Pending'", (sid,)).fetchone()['c']
    conn.close()
    return render_template('student/dashboard.html', announcements=announcements, student=student,
                           att_pct=att_pct, att_present=att_present, att_total=att_total, pending_apps=pending_apps)

@app.route('/student/attendance')
@login_required('student')
def student_attendance():
    sid, cid = get_student_id()
    conn = get_db()
    records = conn.execute("""
        SELECT a.*, sub.subject_name FROM attendance a
        LEFT JOIN subjects sub ON a.subject_id=sub.id
        WHERE a.student_id=? ORDER BY a.date DESC
    """, (sid,)).fetchall()
    conn.close()
    return render_template('student/attendance.html', records=records)

@app.route('/student/marks')
@login_required('student')
def student_marks():
    sid, cid = get_student_id()
    conn = get_db()
    marks = conn.execute("""
        SELECT m.*, sub.subject_name FROM marks m
        LEFT JOIN subjects sub ON m.subject_id=sub.id
        WHERE m.student_id=? ORDER BY sub.subject_name
    """, (sid,)).fetchall()
    conn.close()
    return render_template('student/marks.html', marks=marks)

@app.route('/student/materials')
@login_required('student')
def student_materials():
    sid, cid = get_student_id()
    conn = get_db()
    materials = conn.execute("""
        SELECT l.*, sub.subject_name FROM lectures l
        LEFT JOIN subjects sub ON l.subject_id=sub.id
        WHERE l.class_id=? ORDER BY l.created_at DESC
    """, (cid,)).fetchall()
    conn.close()
    return render_template('student/materials.html', materials=materials)

@app.route('/student/applications', methods=['GET', 'POST'])
@login_required('student')
def student_applications():
    sid, cid = get_student_id()
    conn = get_db()
    if request.method == 'POST':
        f = request.form
        conn.execute("INSERT INTO applications (student_id, subject, details, send_to) VALUES (?,?,?,?)",
                     (sid, f['subject'], f['details'], f['send_to']))
        conn.commit()
        flash('Application submitted!', 'success')
    apps = conn.execute("SELECT * FROM applications WHERE student_id=? ORDER BY created_at DESC", (sid,)).fetchall()
    conn.close()
    return render_template('student/applications.html', applications=apps)

@app.route('/student/qa', methods=['GET', 'POST'])
@login_required('student')
def student_qa():
    sid, cid = get_student_id()
    conn = get_db()
    teachers = conn.execute("""
        SELECT DISTINCT t.id, t.name, sub.id as subject_id, sub.subject_name
        FROM teacher_subjects ts
        JOIN teachers t ON ts.teacher_id=t.id
        JOIN subjects sub ON ts.subject_id=sub.id
        WHERE ts.class_id=?
    """, (cid,)).fetchall()
    if request.method == 'POST':
        f = request.form
        conn.execute("INSERT INTO qa (student_id, teacher_id, subject_id, question) VALUES (?,?,?,?)",
                     (sid, f['teacher_id'], f['subject_id'], f['question']))
        conn.commit()
        flash('Question submitted!', 'success')
    questions = conn.execute("""
        SELECT q.*, t.name as teacher_name, sub.subject_name
        FROM qa q LEFT JOIN teachers t ON q.teacher_id=t.id
        LEFT JOIN subjects sub ON q.subject_id=sub.id
        WHERE q.student_id=? ORDER BY q.created_at DESC
    """, (sid,)).fetchall()
    conn.close()
    return render_template('student/qa.html', teachers=teachers, questions=questions)

if __name__ == '__main__':
    init_db()
    app.run(debug=True, host='0.0.0.0', port=5000)
