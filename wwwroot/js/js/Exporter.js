var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/FruitCustomer/Exporter/GetAll"
        },
        "columns": [
            { "data": "keyCode", "width": "16%" },
            { "data": "name", "width": "16%" },
            { "data": "email", "width": "16%" },

            {
                "data": {
                    emailActive: "emailActive"
                },

                "render": function (data) {

                    if (data.emailActive == true) {
                        return `
                            <div class="text-center">
                                <a onclick=IsActive("/FruitCustomer/Exporter/DeactivateSendUser/${data.id}") class="btn btn-success btn-lg btn-block text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="Click to Turn Off">
                                  <i class="fas fa-power-off"></i>
                                </a>
                            </div>
                           `;
                    }

                    else {
                        return `
                            <div class="text-center">
                                <a onclick=IsActive("/FruitCustomer/Exporter/DeactivateSendUser/${data.id}") class="btn btn-danger btn-lg btn-block text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="Click to Turn On">
                                 <i class="fas fa-power-off"></i>
                                </a>
                            </div>
                           `;
                    }


                }, "width": "10%"

            },
            {
                "data": {
                    isActive: "isActive"
                },

                "render": function (data) {

                    if (data.isActive == true) {
                        return `
                            <div class="text-center">
                                <a onclick=IsActive("/FruitCustomer/Exporter/Deactivate/${data.id}") class="btn btn-success btn-lg btn-block text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="Click to Turn Off">
                                  <i class="fas fa-power-off"></i>
                                </a>
                            </div>
                           `;
                    }

                    else {
                        return `
                            <div class="text-center">
                                <a onclick=IsActive("/FruitCustomer/Exporter/Deactivate/${data.id}") class="btn btn-danger btn-lg btn-block text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="Click to Turn On">
                                 <i class="fas fa-power-off"></i>
                                </a>
                            </div>
                           `;
                    }


                }, "width": "10%"

            }
        ]
    });
}

function IsActive(url) {
    swal({
        title: "Are you sure?",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}

function Delete(url) {
    swal({
        title: "Are you sure you want to Delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}