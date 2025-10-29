var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'email', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            {
                // CRITICAL FIX: The data source is now the 'company' object itself.
                data: 'company',
                "render": function (data) {
                    // The render function now checks if the company object (data) exists
                    // before trying to access its 'name' property.
                    if (data) {
                        return data.name;
                    }
                    return ""; // Otherwise, display an empty string
                },
                "width": "15%"
            },
            { data: 'role', "width": "15%" },
            {
                data: 'id',
                "render": function (data, type, row) {
                    // We use the 'row' object to access other properties like 'lockoutEnd'
                    var today = new Date().getTime();
                    var lockout = new Date(row.lockoutEnd).getTime();

                    // Corrected Logic: If lockout date is in the future, the user is locked.
                    if (lockout > today) {
                        // Display the "Unlock" button for locked users
                        return `
                        <div class="text-center">
                             <a onclick=LockUnlock('${data}') class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                                <i class="bi bi-unlock-fill"></i>  Unlock
                            </a>
                            <a href="/admin/user/roleManagement?userId=${data}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                <i class="bi bi-pencil-square"></i> Permission
                            </a>
                        </div>
                        `
                    }
                    else {
                        // Display the "Lock" button for active users
                        return `
                        <div class="text-center">
                             <a onclick=LockUnlock('${data}') class="btn btn-danger text-white" style="cursor:pointer; width:100px;">
                                <i class="bi bi-lock-fill"></i>  Lock
                            </a>
                            <a href="/admin/user/roleManagement?userId=${data}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                <i class="bi bi-pencil-square"></i> Permission
                            </a>
                        </div>
                        `
                    }
                },
                "width": "25%"
            }
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}
