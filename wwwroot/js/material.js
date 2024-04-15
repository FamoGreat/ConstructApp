
var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#dataTable').DataTable({
        "ajax": {
            "url": '/Material/GetAll',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": 'materialCode', "width": "30%" },
            { "data": 'materialName', "width": "30%" },
            { "data": 'materialUOM', "width": "20%" },
            {
                "data": 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                            <a href="/Material/Edit?id=${data}" class="m-2 btn btn-sm btn-primary"><i class="fas fa-edit"></i> Edit</a>
                            <a onClick=Delete('/Material/Delete/${data}') class="m-2 btn btn-sm btn-danger"><i class="fas fa-trash-alt"></i> Delete</a>
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


