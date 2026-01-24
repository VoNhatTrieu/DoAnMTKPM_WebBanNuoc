/**
 * ADMIN DASHBOARD JAVASCRIPT
 * Web Bán Nước - Admin Panel
 * Responsive & Interactive Features
 */

(function() {
    'use strict';

    // Global Admin Object
    window.AdminDashboard = {
        // Configuration
        config: {
            apiBaseUrl: '/api',
            dateFormat: 'DD/MM/YYYY',
            timeFormat: 'HH:mm',
            currency: '₫'
        },

        // Initialize all components
        init: function() {
            this.setupEventListeners();
            this.setupTooltips();
            this.setupDatePickers();
            this.checkNotifications();
        },

        // Event Listeners
        setupEventListeners: function() {
            // Add any global event listeners here
            console.log('Admin Dashboard initialized');
        },

        // Bootstrap Tooltips
        setupTooltips: function() {
            if (typeof bootstrap !== 'undefined') {
                var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
                tooltipTriggerList.map(function(tooltipTriggerEl) {
                    return new bootstrap.Tooltip(tooltipTriggerEl);
                });
            }
        },

        // Date Pickers Setup
        setupDatePickers: function() {
            // Set today's date as default for date inputs
            var today = new Date().toISOString().split('T')[0];
            var dateInputs = document.querySelectorAll('input[type="date"]');
            dateInputs.forEach(function(input) {
                if (!input.value) {
                    input.value = today;
                }
            });
        },

        // Check for new notifications
        checkNotifications: function() {
            // Implement notification checking logic
            // This would typically make an AJAX call to check for new orders, messages, etc.
        },

        // Utility Functions
        utils: {
            // Format currency
            formatCurrency: function(amount) {
                return new Intl.NumberFormat('vi-VN', {
                    style: 'currency',
                    currency: 'VND'
                }).format(amount);
            },

            // Format date
            formatDate: function(date) {
                return new Date(date).toLocaleDateString('vi-VN');
            },

            // Format datetime
            formatDateTime: function(date) {
                return new Date(date).toLocaleString('vi-VN');
            },

            // Show loading spinner
            showLoading: function(element) {
                element.innerHTML = '<div class="text-center"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>';
            },

            // Show success message
            showSuccess: function(message) {
                this.showAlert('success', message);
            },

            // Show error message
            showError: function(message) {
                this.showAlert('danger', message);
            },

            // Show alert
            showAlert: function(type, message) {
                var alertHtml = '<div class="alert alert-' + type + ' alert-dismissible fade show" role="alert">' +
                    message +
                    '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>' +
                    '</div>';
                
                var container = document.querySelector('.content-wrapper');
                if (container) {
                    var alertDiv = document.createElement('div');
                    alertDiv.innerHTML = alertHtml;
                    container.insertBefore(alertDiv.firstChild, container.firstChild);
                    
                    // Auto-dismiss after 5 seconds
                    setTimeout(function() {
                        var alert = container.querySelector('.alert');
                        if (alert) {
                            alert.remove();
                        }
                    }, 5000);
                }
            },

            // Confirm dialog
            confirm: function(message, callback) {
                if (window.confirm(message)) {
                    callback();
                }
            },

            // Debounce function for search inputs
            debounce: function(func, wait) {
                var timeout;
                return function executedFunction() {
                    var context = this;
                    var args = arguments;
                    var later = function() {
                        timeout = null;
                        func.apply(context, args);
                    };
                    clearTimeout(timeout);
                    timeout = setTimeout(later, wait);
                };
            }
        },

        // AJAX Helper
        ajax: {
            // GET request
            get: function(url, successCallback, errorCallback) {
                fetch(url, {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                })
                .then(response => response.json())
                .then(data => {
                    if (successCallback) successCallback(data);
                })
                .catch(error => {
                    console.error('GET Error:', error);
                    if (errorCallback) errorCallback(error);
                });
            },

            // POST request
            post: function(url, data, successCallback, errorCallback) {
                fetch(url, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(data)
                })
                .then(response => response.json())
                .then(data => {
                    if (successCallback) successCallback(data);
                })
                .catch(error => {
                    console.error('POST Error:', error);
                    if (errorCallback) errorCallback(error);
                });
            },

            // PUT request
            put: function(url, data, successCallback, errorCallback) {
                fetch(url, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(data)
                })
                .then(response => response.json())
                .then(data => {
                    if (successCallback) successCallback(data);
                })
                .catch(error => {
                    console.error('PUT Error:', error);
                    if (errorCallback) errorCallback(error);
                });
            },

            // DELETE request
            delete: function(url, successCallback, errorCallback) {
                fetch(url, {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                })
                .then(response => response.json())
                .then(data => {
                    if (successCallback) successCallback(data);
                })
                .catch(error => {
                    console.error('DELETE Error:', error);
                    if (errorCallback) errorCallback(error);
                });
            }
        },

        // Chart helpers
        charts: {
            // Default chart options
            defaultOptions: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true,
                        position: 'bottom'
                    }
                }
            },

            // Create line chart
            createLineChart: function(ctx, labels, data, label) {
                return new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: label,
                            data: data,
                            borderColor: '#4e73df',
                            backgroundColor: 'rgba(78, 115, 223, 0.1)',
                            tension: 0.4,
                            fill: true
                        }]
                    },
                    options: this.defaultOptions
                });
            },

            // Create bar chart
            createBarChart: function(ctx, labels, data, label) {
                return new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: label,
                            data: data,
                            backgroundColor: '#4e73df'
                        }]
                    },
                    options: this.defaultOptions
                });
            },

            // Create pie chart
            createPieChart: function(ctx, labels, data) {
                return new Chart(ctx, {
                    type: 'pie',
                    data: {
                        labels: labels,
                        datasets: [{
                            data: data,
                            backgroundColor: [
                                '#4e73df',
                                '#1cc88a',
                                '#36b9cc',
                                '#f6c23e',
                                '#e74a3b'
                            ]
                        }]
                    },
                    options: this.defaultOptions
                });
            }
        },

        // Table helpers
        table: {
            // Sort table
            sortTable: function(tableId, columnIndex) {
                var table = document.getElementById(tableId);
                var tbody = table.querySelector('tbody');
                var rows = Array.from(tbody.querySelectorAll('tr'));
                
                rows.sort(function(a, b) {
                    var aValue = a.cells[columnIndex].textContent;
                    var bValue = b.cells[columnIndex].textContent;
                    return aValue.localeCompare(bValue);
                });
                
                rows.forEach(function(row) {
                    tbody.appendChild(row);
                });
            },

            // Filter table
            filterTable: function(tableId, filterValue) {
                var table = document.getElementById(tableId);
                var tbody = table.querySelector('tbody');
                var rows = tbody.querySelectorAll('tr');
                
                rows.forEach(function(row) {
                    var text = row.textContent.toLowerCase();
                    if (text.includes(filterValue.toLowerCase())) {
                        row.style.display = '';
                    } else {
                        row.style.display = 'none';
                    }
                });
            }
        },

        // Form validation
        validation: {
            // Validate email
            isValidEmail: function(email) {
                var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                return re.test(email);
            },

            // Validate phone (Vietnamese format)
            isValidPhone: function(phone) {
                var re = /^(0|\+84)[0-9]{9}$/;
                return re.test(phone);
            },

            // Validate required field
            isRequired: function(value) {
                return value !== null && value !== undefined && value.trim() !== '';
            },

            // Show validation error
            showError: function(inputElement, message) {
                var errorDiv = inputElement.parentElement.querySelector('.invalid-feedback');
                if (!errorDiv) {
                    errorDiv = document.createElement('div');
                    errorDiv.className = 'invalid-feedback';
                    inputElement.parentElement.appendChild(errorDiv);
                }
                errorDiv.textContent = message;
                inputElement.classList.add('is-invalid');
            },

            // Clear validation error
            clearError: function(inputElement) {
                inputElement.classList.remove('is-invalid');
                var errorDiv = inputElement.parentElement.querySelector('.invalid-feedback');
                if (errorDiv) {
                    errorDiv.remove();
                }
            }
        }
    };

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            AdminDashboard.init();
        });
    } else {
        AdminDashboard.init();
    }

})();

// Export for module usage if needed
if (typeof module !== 'undefined' && module.exports) {
    module.exports = AdminDashboard;
}
