const API = 'https://localhost:7018/api/employees';
let currentPage = 1;
const pageSize = 5;

// ── AUTH HELPERS ──────────────────────────────────────────────────────────────
function getToken() {
    return localStorage.getItem('emp_token');
}

function authHeaders() {
    return {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${getToken()}`
    };
}

function checkAuth() {
    if (!getToken()) {
        window.location.href = '/login.html';
    }
}

function logout() {
    localStorage.removeItem('emp_token');
    localStorage.removeItem('emp_username');
    localStorage.removeItem('emp_role');
    window.location.href = '/login.html';
}

// ── NAVIGATION ────────────────────────────────────────────────────────────────
function showSection(name, fromNav = false) {
    ['dashboard', 'employees', 'add'].forEach(s => {
        document.getElementById(`section-${s}`).style.display = 'none';
    });
    document.getElementById(`section-${name}`).style.display = 'block';

    document.querySelectorAll('.nav-link').forEach(l => l.classList.remove('active'));
    const navIndex = { dashboard: 0, employees: 1, add: 2 };
    const navLinks = document.querySelectorAll('.nav-link');
    if (navLinks[navIndex[name]]) {
        navLinks[navIndex[name]].classList.add('active');
    }

    if (name === 'dashboard') loadStats();
    if (name === 'employees') { currentPage = 1; loadEmployees(); }
    if (name === 'add' && fromNav) resetForm();
}

// ── LOAD STATS ────────────────────────────────────────────────────────────────
async function loadStats() {
    const res = await fetch(`${API}/stats`, {
        headers: authHeaders()
    });

    if (res.status === 401) { logout(); return; }

    const data = await res.json();

    document.getElementById('stat-total').textContent = data.total;
    document.getElementById('stat-active').textContent = data.active;
    document.getElementById('stat-inactive').textContent = data.inactive;

    const tbody = document.querySelector('#dept-table tbody');
    tbody.innerHTML = data.byDepartment
        .map(d => `<tr><td>${d.department}</td><td>${d.count}</td></tr>`)
        .join('');
}

// ── LOAD EMPLOYEES ────────────────────────────────────────────────────────────
async function loadEmployees() {
    const search = document.getElementById('search-input').value;
    const dept = document.getElementById('filter-dept').value;
    const status = document.getElementById('filter-status').value;

    let url = `${API}?page=${currentPage}&pageSize=${pageSize}`;
    if (search) url += `&search=${encodeURIComponent(search)}`;
    if (dept) url += `&department=${encodeURIComponent(dept)}`;
    if (status !== '') url += `&isActive=${status}`;

    const res = await fetch(url, { headers: authHeaders() });

    if (res.status === 401) { logout(); return; }

    const data = await res.json();
    renderTable(data.data);
    renderPagination(data.totalPages);
}

// ── RENDER TABLE ──────────────────────────────────────────────────────────────
function renderTable(employees) {
    const tbody = document.getElementById('emp-table-body');

    if (employees.length === 0) {
        tbody.innerHTML = `<tr><td colspan="8" style="text-align:center;color:#94a3b8;padding:32px">No employees found.</td></tr>`;
        return;
    }

    tbody.innerHTML = employees.map(e => `
        <tr>
            <td><strong>${e.firstName} ${e.lastName}</strong></td>
            <td>${e.email}</td>
            <td>${e.department}</td>
            <td>${e.designation}</td>
            <td>₹${Number(e.salary).toLocaleString()}</td>
            <td>${new Date(e.dateOfJoining).toLocaleDateString()}</td>
            <td><span class="badge ${e.isActive ? 'active' : 'inactive'}">${e.isActive ? 'Active' : 'Inactive'}</span></td>
            <td style="display:flex;gap:6px;flex-wrap:wrap">
                <button class="btn btn-view btn-sm"   onclick="viewEmployee(${e.employeeId})">👁 View</button>
                <button class="btn btn-edit btn-sm"   onclick="editEmployee(${e.employeeId})">✏️ Edit</button>
                <button class="btn btn-toggle btn-sm" onclick="toggleStatus(${e.employeeId})">🔄 Toggle</button>
                <button class="btn btn-delete btn-sm" onclick="deleteEmployee(${e.employeeId})">🗑 Delete</button>
            </td>
        </tr>
    `).join('');
}

// ── PAGINATION ────────────────────────────────────────────────────────────────
function renderPagination(totalPages) {
    const container = document.getElementById('pagination');
    container.innerHTML = '';

    for (let i = 1; i <= totalPages; i++) {
        const btn = document.createElement('button');
        btn.className = `page-btn ${i === currentPage ? 'active' : ''}`;
        btn.textContent = i;
        btn.onclick = () => { currentPage = i; loadEmployees(); };
        container.appendChild(btn);
    }
}

// ── VIEW EMPLOYEE (MODAL) ─────────────────────────────────────────────────────
async function viewEmployee(id) {
    const res = await fetch(`${API}/${id}`, { headers: authHeaders() });
    const e = await res.json();

    document.getElementById('modal-name').textContent = `${e.firstName} ${e.lastName}`;
    document.getElementById('modal-body').innerHTML = `
        <div class="modal-row"><span>Email</span>       <span>${e.email}</span></div>
        <div class="modal-row"><span>Phone</span>       <span>${e.phone}</span></div>
        <div class="modal-row"><span>Department</span>  <span>${e.department}</span></div>
        <div class="modal-row"><span>Designation</span> <span>${e.designation}</span></div>
        <div class="modal-row"><span>Salary</span>      <span>₹${Number(e.salary).toLocaleString()}</span></div>
        <div class="modal-row"><span>Joined</span>      <span>${new Date(e.dateOfJoining).toLocaleDateString()}</span></div>
        <div class="modal-row"><span>Status</span>      <span class="badge ${e.isActive ? 'active' : 'inactive'}">${e.isActive ? 'Active' : 'Inactive'}</span></div>
    `;
    document.getElementById('modal-overlay').style.display = 'flex';
}

function closeModal() {
    document.getElementById('modal-overlay').style.display = 'none';
}

// ── RESET FORM ────────────────────────────────────────────────────────────────
function resetForm() {
    document.getElementById('emp-id').value = '';
    document.getElementById('emp-form').reset();
    document.getElementById('form-title').textContent = 'Add Employee';
    document.getElementById('submit-btn').textContent = '➕ Add Employee';
    document.getElementById('form-error').textContent = '';
}

// ── EDIT EMPLOYEE ─────────────────────────────────────────────────────────────
async function editEmployee(id) {
    const res = await fetch(`${API}/${id}`, { headers: authHeaders() });
    const e = await res.json();

    showSection('add', false);

    document.getElementById('form-title').textContent = 'Edit Employee';
    document.getElementById('submit-btn').textContent = '💾 Save Changes';
    document.getElementById('form-error').textContent = '';

    document.getElementById('emp-id').value = e.employeeId;
    document.getElementById('firstName').value = e.firstName;
    document.getElementById('lastName').value = e.lastName;
    document.getElementById('email').value = e.email;
    document.getElementById('phone').value = e.phone;
    document.getElementById('department').value = e.department;
    document.getElementById('designation').value = e.designation;
    document.getElementById('salary').value = e.salary;
    document.getElementById('dateOfJoining').value = e.dateOfJoining.split('T')[0];
}

// ── SUBMIT FORM (ADD or EDIT) ─────────────────────────────────────────────────
async function submitForm(event) {
    event.preventDefault();
    const id = document.getElementById('emp-id').value;

    const payload = {
        firstName: document.getElementById('firstName').value,
        lastName: document.getElementById('lastName').value,
        email: document.getElementById('email').value,
        phone: document.getElementById('phone').value,
        department: document.getElementById('department').value,
        designation: document.getElementById('designation').value,
        salary: parseFloat(document.getElementById('salary').value),
        dateOfJoining: document.getElementById('dateOfJoining').value,
    };

    const url = id ? `${API}/${id}` : API;
    const method = id ? 'PUT' : 'POST';

    const res = await fetch(url, {
        method,
        headers: authHeaders(),
        body: JSON.stringify(payload)
    });

    if (res.status === 401) { logout(); return; }

    if (res.ok) {
        resetForm();
        showSection('employees', false);
    } else {
        const err = await res.json();
        document.getElementById('form-error').textContent =
            err.message || err.Message || 'Something went wrong.';
    }
}

// ── TOGGLE STATUS ─────────────────────────────────────────────────────────────
async function toggleStatus(id) {
    await fetch(`${API}/${id}/toggle-status`, {
        method: 'PATCH',
        headers: authHeaders()
    });
    loadEmployees();
}

// ── DELETE ────────────────────────────────────────────────────────────────────
async function deleteEmployee(id) {
    if (!confirm('Are you sure you want to delete this employee?')) return;
    await fetch(`${API}/${id}`, {
        method: 'DELETE',
        headers: authHeaders()
    });
    loadEmployees();
}

// ── EXPORT HELPERS ────────────────────────────────────────────────────────────
async function fetchAllEmployees() {
    const search = document.getElementById('search-input').value;
    const dept = document.getElementById('filter-dept').value;
    const status = document.getElementById('filter-status').value;

    let url = `${API}?page=1&pageSize=10000`;
    if (search) url += `&search=${encodeURIComponent(search)}`;
    if (dept) url += `&department=${encodeURIComponent(dept)}`;
    if (status !== '') url += `&isActive=${status}`;

    const res = await fetch(url, { headers: authHeaders() });
    const data = await res.json();
    return data.data;
}

function formatEmployeesForExport(employees) {
    return employees.map(e => ({
        'First Name': e.firstName,
        'Last Name': e.lastName,
        'Email': e.email,
        'Phone': e.phone,
        'Department': e.department,
        'Designation': e.designation,
        'Salary (₹)': e.salary,
        'Date of Joining': new Date(e.dateOfJoining).toLocaleDateString(),
        'Status': e.isActive ? 'Active' : 'Inactive'
    }));
}

async function exportToExcel() {
    const employees = await fetchAllEmployees();
    if (employees.length === 0) { alert('No employees to export!'); return; }

    const formatted = formatEmployeesForExport(employees);
    const wb = XLSX.utils.book_new();
    const ws = XLSX.utils.json_to_sheet(formatted);

    ws['!cols'] = [
        { wch: 15 }, { wch: 15 }, { wch: 28 }, { wch: 15 },
        { wch: 15 }, { wch: 20 }, { wch: 15 }, { wch: 18 }, { wch: 10 }
    ];

    XLSX.utils.book_append_sheet(wb, ws, 'Employees');
    XLSX.writeFile(wb, `Employees_${new Date().toISOString().split('T')[0]}.xlsx`);
}

async function exportToCSV() {
    const employees = await fetchAllEmployees();
    if (employees.length === 0) { alert('No employees to export!'); return; }

    const formatted = formatEmployeesForExport(employees);
    const wb = XLSX.utils.book_new();
    const ws = XLSX.utils.json_to_sheet(formatted);
    XLSX.utils.book_append_sheet(wb, ws, 'Employees');
    XLSX.writeFile(wb, `Employees_${new Date().toISOString().split('T')[0]}.csv`);
}

// ── INIT ──────────────────────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
    checkAuth();
    document.getElementById('sidebar-username').textContent =
        localStorage.getItem('emp_username') || 'Admin';
    loadStats();
});