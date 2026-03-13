-- ROLES
CREATE TABLE roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100)
);

-- USERS
CREATE TABLE users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(100),
    password_hash NVARCHAR(255),
    role_id INT,
    is_active BIT
);

ALTER TABLE users
ADD CONSTRAINT FK_users_roles
FOREIGN KEY (role_id) REFERENCES roles(id);

--------------------------------------------------

-- DEPARTMENTS
CREATE TABLE departments (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(150),
    description NVARCHAR(255)
);

-- POSITIONS
CREATE TABLE positions (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(150)
);

--------------------------------------------------

-- EMPLOYEES
CREATE TABLE employees (
    id INT IDENTITY(1,1) PRIMARY KEY,
    code NVARCHAR(50),
    fullname NVARCHAR(200),
    gender BIT,
    date_of_birth DATE,
    phone NVARCHAR(20),
    email NVARCHAR(150),
    address NVARCHAR(255),
    hire_date DATE,
    department_id INT,
    position_id INT,
    is_active BIT
);

ALTER TABLE employees
ADD CONSTRAINT FK_employees_departments
FOREIGN KEY (department_id) REFERENCES departments(id);

ALTER TABLE employees
ADD CONSTRAINT FK_employees_positions
FOREIGN KEY (position_id) REFERENCES positions(id);

--------------------------------------------------

-- CONTRACTS
CREATE TABLE contracts (
    id INT IDENTITY(1,1) PRIMARY KEY,
    employee_id INT,
    contract_type NVARCHAR(100),
    start_date DATE,
    end_date DATE,
    base_salary DECIMAL(18,2)
);

ALTER TABLE contracts
ADD CONSTRAINT FK_contracts_employees
FOREIGN KEY (employee_id) REFERENCES employees(id);

--------------------------------------------------

-- SHIFTS
CREATE TABLE shifts (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100),
    start_time TIME,
    end_time TIME,
    standard_hours INT
);

--------------------------------------------------

-- EMPLOYEE SHIFTS
CREATE TABLE employee_shifts (
    id INT IDENTITY(1,1) PRIMARY KEY,
    employee_id INT,
    shift_id INT,
    from_date DATE,
    to_date DATE
);

ALTER TABLE employee_shifts
ADD CONSTRAINT FK_employee_shifts_employee
FOREIGN KEY (employee_id) REFERENCES employees(id);

ALTER TABLE employee_shifts
ADD CONSTRAINT FK_employee_shifts_shift
FOREIGN KEY (shift_id) REFERENCES shifts(id);

--------------------------------------------------

-- ATTENDANCES
CREATE TABLE attendances (
    id INT IDENTITY(1,1) PRIMARY KEY,
    employee_id INT,
    shift_id INT,
    work_date DATE,
    check_in DATETIME,
    check_out DATETIME,
    total_hours FLOAT,
    status NVARCHAR(50)
);

ALTER TABLE attendances
ADD CONSTRAINT FK_attendances_employee
FOREIGN KEY (employee_id) REFERENCES employees(id);

ALTER TABLE attendances
ADD CONSTRAINT FK_attendances_shift
FOREIGN KEY (shift_id) REFERENCES shifts(id);

--------------------------------------------------

-- LEAVE TYPES
CREATE TABLE leave_types (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100),
    is_paid BIT
);

-- LEAVE REQUESTS
CREATE TABLE leave_requests (
    id INT IDENTITY(1,1) PRIMARY KEY,
    employee_id INT,
    leave_type_id INT,
    from_date DATE,
    to_date DATE,
    reason NVARCHAR(255),
    status NVARCHAR(50),
    created_date DATETIME
);

ALTER TABLE leave_requests
ADD CONSTRAINT FK_leave_requests_employee
FOREIGN KEY (employee_id) REFERENCES employees(id);

ALTER TABLE leave_requests
ADD CONSTRAINT FK_leave_requests_type
FOREIGN KEY (leave_type_id) REFERENCES leave_types(id);

--------------------------------------------------

-- OVERTIMES
CREATE TABLE overtimes (
    id INT IDENTITY(1,1) PRIMARY KEY,
    employee_id INT,
    work_date DATE,
    hours FLOAT,
    rate FLOAT
);

ALTER TABLE overtimes
ADD CONSTRAINT FK_overtimes_employee
FOREIGN KEY (employee_id) REFERENCES employees(id);

--------------------------------------------------

-- REWARD TYPES
CREATE TABLE reward_types (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100),
    type NVARCHAR(50)
);

-- EMPLOYEE REWARDS
CREATE TABLE employee_rewards (
    id INT IDENTITY(1,1) PRIMARY KEY,
    employee_id INT,
    reward_type_id INT,
    amount FLOAT,
    month INT,
    year INT,
    note NVARCHAR(255)
);

ALTER TABLE employee_rewards
ADD CONSTRAINT FK_employee_rewards_employee
FOREIGN KEY (employee_id) REFERENCES employees(id);

ALTER TABLE employee_rewards
ADD CONSTRAINT FK_employee_rewards_type
FOREIGN KEY (reward_type_id) REFERENCES reward_types(id);

--------------------------------------------------

-- PAYROLLS
CREATE TABLE payrolls (
    id INT IDENTITY(1,1) PRIMARY KEY,
    employee_id INT,
    month INT,
    year INT,
    total_work_days FLOAT,
    total_hours FLOAT,
    overtime_hours FLOAT,
    gross_salary FLOAT,
    net_salary FLOAT,
    created_date DATETIME
);

ALTER TABLE payrolls
ADD CONSTRAINT FK_payrolls_employee
FOREIGN KEY (employee_id) REFERENCES employees(id);

--------------------------------------------------

-- PAYROLL DETAILS
CREATE TABLE payroll_details (
    id INT IDENTITY(1,1) PRIMARY KEY,
    payroll_id INT,
    type NVARCHAR(50),
    description NVARCHAR(255),
    amount FLOAT
);

ALTER TABLE payroll_details
ADD CONSTRAINT FK_payroll_details_payroll
FOREIGN KEY (payroll_id) REFERENCES payrolls(id);

--------------------------------------------------

-- HOLIDAYS
CREATE TABLE holidays (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100),
    holiday_date DATE,
    is_paid BIT,
    description NVARCHAR(255)
);