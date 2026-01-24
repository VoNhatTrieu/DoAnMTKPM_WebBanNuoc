// Admin Customers Management - Realtime Data từ API
$(document).ready(function () {
    loadCustomers();
});

// Load danh sách khách hàng từ API
async function loadCustomers() {
    try {
        const response = await adminAPI.get('/admin/customers');
        if (response.success && response.data) {
            renderCustomerStats(response.data);
            renderCustomersTable(response.data);
        } else {
            showEmptyState();
        }
    } catch (error) {
        console.error('Lỗi khi tải khách hàng:', error);
        showErrorState('Không thể tải dữ liệu khách hàng. Vui lòng thử lại.');
    }
}

// Render customer stats cards
function renderCustomerStats(customers) {
    const totalCustomers = customers.length;
    const totalSpent = customers.reduce((sum, c) => sum + c.totalSpent, 0);
    const avgSpent = totalCustomers > 0 ? totalSpent / totalCustomers : 0;
    
    // Calculate new customers this month
    const now = new Date();
    const firstDayOfMonth = new Date(now.getFullYear(), now.getMonth(), 1);
    const newCustomers = customers.filter(c => {
        const createdDate = new Date(c.createdAt);
        return createdDate >= firstDayOfMonth;
    }).length;

    // Active customers (have at least 1 order)
    const activeCustomers = customers.filter(c => c.totalOrders > 0).length;

    const html = `
        <div class="col-xl-3 col-md-6 mb-3">
            <div class="card border-left-primary shadow h-100 py-2">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs font-weight-bold text-primary text-uppercase mb-1">Tổng KH</div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">${totalCustomers.toLocaleString()}</div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-users fa-2x text-gray-300"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-xl-3 col-md-6 mb-3">
            <div class="card border-left-success shadow h-100 py-2">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs font-weight-bold text-success text-uppercase mb-1">KH mới (tháng)</div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">${newCustomers.toLocaleString()}</div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-user-plus fa-2x text-gray-300"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-xl-3 col-md-6 mb-3">
            <div class="card border-left-info shadow h-100 py-2">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs font-weight-bold text-info text-uppercase mb-1">KH hoạt động</div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">${activeCustomers.toLocaleString()}</div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-user-check fa-2x text-gray-300"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-xl-3 col-md-6 mb-3">
            <div class="card border-left-warning shadow h-100 py-2">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs font-weight-bold text-warning text-uppercase mb-1">Giá trị TB/KH</div>
                            <div class="h5 mb-0 font-weight-bold text-gray-800">${formatCurrency(avgSpent)}</div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-coins fa-2x text-gray-300"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;

    $('#customerStatsCards').html(html);
}

// Render customers table
function renderCustomersTable(customers) {
    const tbody = $('#customersTableBody');
    
    if (!customers || customers.length === 0) {
        showEmptyState();
        return;
    }

    let html = '';
    customers.forEach(customer => {
        const initials = getInitials(customer.fullName);
        const statusBadge = getCustomerStatusBadge(customer.totalOrders);
        
        html += `
            <tr data-customer-id="${customer.id}">
                <td>
                    <div class="d-flex align-items-center">
                        <div class="customer-avatar me-3">${initials}</div>
                        <div>
                            <strong>${escapeHtml(customer.fullName)}</strong><br>
                            <small class="text-muted">ID: #CUS${String(customer.id).padStart(3, '0')}</small>
                        </div>
                    </div>
                </td>
                <td>
                    ${escapeHtml(customer.email)}<br>
                    <small class="text-muted">Ngày tham gia: ${formatDate(customer.createdAt)}</small>
                </td>
                <td>${formatDate(customer.createdAt)}</td>
                <td><strong>${customer.totalOrders}</strong></td>
                <td><strong class="text-success">${formatCurrency(customer.totalSpent)}</strong></td>
                <td>${statusBadge}</td>
                <td>
                    <div class="btn-group-sm">
                        <button class="btn btn-sm btn-info" onclick="viewCustomer(${customer.id})" title="Xem chi tiết">
                            <i class="fas fa-eye"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
    });

    tbody.html(html);
}

// Get initials from name
function getInitials(name) {
    if (!name) return '?';
    const parts = name.trim().split(' ');
    if (parts.length === 1) return parts[0].substring(0, 2).toUpperCase();
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
}

// Get customer status badge
function getCustomerStatusBadge(totalOrders) {
    if (totalOrders === 0) {
        return '<span class="customer-status status-inactive">Chưa mua hàng</span>';
    } else if (totalOrders >= 10) {
        return '<span class="customer-status status-active">VIP</span>';
    } else {
        return '<span class="customer-status status-active">Hoạt động</span>';
    }
}

// Show empty state
function showEmptyState() {
    $('#customersTableBody').html(`
        <tr>
            <td colspan="7" class="text-center py-5">
                <i class="fas fa-users-slash fa-3x text-muted mb-3"></i>
                <p class="text-muted">Chưa có khách hàng nào</p>
            </td>
        </tr>
    `);
    
    $('#customerStatsCards').html(`
        <div class="col-12 text-center py-3">
            <p class="text-muted">Chưa có dữ liệu khách hàng</p>
        </div>
    `);
}

// Show error state
function showErrorState(message) {
    $('#customersTableBody').html(`
        <tr>
            <td colspan="7" class="text-center py-5">
                <i class="fas fa-exclamation-triangle fa-3x text-warning mb-3"></i>
                <p class="text-muted">${message}</p>
                <button class="btn btn-primary" onclick="loadCustomers()">
                    <i class="fas fa-redo"></i> Thử lại
                </button>
            </td>
        </tr>
    `);
}

// View customer details
function viewCustomer(id) {
    adminAPI.get(`/admin/customers/${id}`)
        .then(response => {
            if (response.success && response.data) {
                showCustomerDetail(response.data);
            }
        })
        .catch(error => {
            adminAPI.showToast('error', 'Không thể tải thông tin khách hàng');
        });
}

// Show customer detail modal
function showCustomerDetail(customer) {
    const initials = getInitials(customer.fullName);
    const modalHtml = `
        <div class="modal fade" id="customerDetailModal" tabindex="-1">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Chi tiết khách hàng</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="text-center mb-4">
                            <div class="customer-avatar mx-auto" style="width: 80px; height: 80px; font-size: 2rem;">
                                ${initials}
                            </div>
                            <h4 class="mt-3">${escapeHtml(customer.fullName)}</h4>
                            <p class="text-muted">Khách hàng #CUS${String(customer.id).padStart(3, '0')}</p>
                        </div>
                        
                        <div class="row mb-3">
                            <div class="col-md-6">
                                <strong>Email:</strong><br>
                                ${escapeHtml(customer.email)}
                            </div>
                            <div class="col-md-6">
                                <strong>Ngày đăng ký:</strong><br>
                                ${formatDate(customer.createdAt)}
                            </div>
                        </div>
                        
                        <div class="row mb-3">
                            <div class="col-md-6">
                                <strong>Tổng đơn hàng:</strong><br>
                                <span class="h5 text-primary">${customer.totalOrders}</span>
                            </div>
                            <div class="col-md-6">
                                <strong>Tổng chi tiêu:</strong><br>
                                <span class="h5 text-success">${formatCurrency(customer.totalSpent)}</span>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    // Remove old modal if exists
    $('#customerDetailModal').remove();
    
    // Append and show new modal
    $('body').append(modalHtml);
    const modal = new bootstrap.Modal(document.getElementById('customerDetailModal'));
    modal.show();
    
    // Clean up on close
    $('#customerDetailModal').on('hidden.bs.modal', function () {
        $(this).remove();
    });
}

// Filter customers
function filterCustomers() {
    const status = $('#filterStatus').val();
    const sortBy = $('#sortBy').val();
    const search = $('#searchCustomer').val().toLowerCase();

    // Get all rows
    let rows = Array.from($('#customersTableBody tr'));
    
    // Filter
    rows = rows.filter(row => {
        const $row = $(row);
        const name = $row.find('td:eq(0) strong').text().toLowerCase();
        const email = $row.find('td:eq(1)').text().toLowerCase();
        const statusText = $row.find('td:eq(5)').text();
        const orders = parseInt($row.find('td:eq(3) strong').text()) || 0;
        
        // Status filter
        if (status === 'active' && orders === 0) return false;
        if (status === 'inactive' && orders > 0) return false;
        
        // Search filter
        if (search && !name.includes(search) && !email.includes(search)) return false;
        
        return true;
    });
    
    // Sort
    rows.sort((a, b) => {
        const $a = $(a);
        const $b = $(b);
        
        switch (sortBy) {
            case 'orders':
                return parseInt($b.find('td:eq(3) strong').text()) - parseInt($a.find('td:eq(3) strong').text());
            case 'spending':
                const spentA = parseFloat($a.find('td:eq(4) strong').text().replace(/[^\d]/g, ''));
                const spentB = parseFloat($b.find('td:eq(4) strong').text().replace(/[^\d]/g, ''));
                return spentB - spentA;
            case 'name':
                return $a.find('td:eq(0) strong').text().localeCompare($b.find('td:eq(0) strong').text());
            case 'newest':
            default:
                return $b.data('customer-id') - $a.data('customer-id');
        }
    });
    
    // Update table
    const tbody = $('#customersTableBody');
    tbody.empty();
    
    if (rows.length === 0) {
        tbody.html(`
            <tr>
                <td colspan="7" class="text-center py-5">
                    <i class="fas fa-search fa-3x text-muted mb-3"></i>
                    <p class="text-muted">Không tìm thấy khách hàng nào</p>
                </td>
            </tr>
        `);
    } else {
        rows.forEach(row => tbody.append(row));
    }
}

// Export customers to Excel
function exportCustomers() {
    adminAPI.showToast('info', 'Tính năng xuất Excel đang được phát triển');
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}
