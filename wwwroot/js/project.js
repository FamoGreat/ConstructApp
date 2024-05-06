
var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#dataTable').DataTable({
        "ajax": {
            "url": '/Project/GetAll',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": 'projectName', "width": "10%" },
            { "data": 'location', "width": "10%" },
            { "data": 'startDate', "width": "10%",  },
            { "data": 'endDate', "width": "10%" },
            { "data": 'createdBy', "width": "10%" },
            { "data": 'createdDate', "width": "10%" },
            { "data": 'totalBudget', "width": "10%" },
            {
                "data": 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                <a href="/Project/Edit?id=${data}" class="m-2 btn btn-sm btn-primary">
                    <div style="display: flex; align-items: center;">
                        <i class="fas fa-edit mr-1"></i>
                    </div>
                </a>
                <a href="/Project/Details?id=${data}" class="m-2 btn btn-sm btn-success">
                    <div style="display: flex; align-items: center;">
                        <i class="fas fa-eye  mr-1"></i>
                    </div>
                </a>
                <a onClick=Delete('/Project/Delete/${data}') class="m-2 btn btn-sm btn-danger">
                    <div style="display: flex; align-items: center;">
                        <i class="fas fa-trash-alt mr-1"></i>
                    </div>
                </a>
            </div>`;
                }
,
                "width": "25%"
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


