﻿


var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#dataTable').DataTable({
        "ajax": {
            "url": '/ProjectMaterial/GetAll',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": 'materialCode', "width": "10%" },
            { "data": 'materialName', "width": "20%" },
            { "data": 'estimatedQuantity', "width": "10%" },
            { "data": 'estimatedCost', "width": "10%" },
            { "data": 'materialUOM', "width": "10%" },
            {
                "data": 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                            <a href="/ProjectMaterial?id=${data}" class="m-2 btn btn-sm btn-primary">Edit</a>
                            <a href="/ProjectMaterial/Details?id=${data}" class="m-2 btn btn-sm btn-success">Details</a>
                            <a onClick=Delete('/ProjectMaterial/Delete/${data}') class="m-2 btn btn-sm btn-danger">Delete</a>
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

