
var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#dataTable').DataTable({
        "ajax": {
            "url": '/User/GetAll',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": 'profileImage', "width": "16%" },
            { "data": 'name', "width": "16%" },
            { "data": 'email', "width": "16%" },
            { "data": 'phoneNumber', "width": "16%" },
            { "data": 'role', "width": "16%" },
            {
                "data": 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                            <a href="/User/Edit?id=${data}" class="m-2 btn btn-sm btn-primary">Edit</a>
                            <a onClick=Delete('/User/Delete/${data}') class="m-2 btn btn-sm btn-danger">Delete</a>
                            </div>`;
                },
                "width": "20%"
            }]
    });
}
function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            });
        }
    });
}


