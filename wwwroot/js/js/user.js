var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/FruitCustomer/User/GetAll"
        }
        , dom: 'Bfrtip',
        buttons: [
            'copyHtml5',
            'excelHtml5',
            'csvHtml5',
            'pdfHtml5',
            {
                text: '<i class="fas fa-redo-alt"></i>',
                action: function (e, dt, node, config) {
                    dt.ajax.reload();
                }
            }
        ],
        "columns": [
            { "data": "firstName", "width": "15%" },
            { "data": "lastName", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "role", "width": "15%" },
            {
                "data": {
                    emailConfirmed: "emailConfirmed"
                },

                "render": function (data) {

                    if (data.emailConfirmed == true) {
                        return `
                            <div class="text-center">
                                <a class="btn btn-success text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="Click to Turn Off">
                                  <i class="fas fa-check"></i>
                                </a>
                            </div>
                           `;
                    }

                    else {
                        return `
                            <div class="text-center">
                                <a  class="btn btn-danger text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="Click to Turn On">
                                 <i class="fas fa-times"></i>
                                </a>
                            </div>
                           `;
                    }


                }, "width": "10%"

            },
            {
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        //user is currently locked
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px;">
                                    <i class="fas fa-lock-open"></i>  Unlock
                                </a>
                            </div>
                           `;
                    }
                    else {
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                                    <i class="fas fa-lock"></i>  Lock
                                </a>
                            </div>
                           `;
                    }

                }, "width": "25%"
            }
        ]
    });
}

function LockUnlock(id) {

    $.ajax({
        type: "POST",
        url: '/FruitCustomer/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
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