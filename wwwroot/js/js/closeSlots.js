var dataTable;

$(document).ready(function () {
    var x = document.getElementById("WarehouseName").value;
    var date = document.getElementById("datePicker").value;
    loadDataTable(x, date);
});


$('#WarehouseName').change(function () {
    var x = document.getElementById("WarehouseName").value;
    var date = document.getElementById("datePicker").value;
    dataTable.destroy();
    loadDataTable(x, date);

});

$('#datePicker').change(function () {
    var x = document.getElementById("WarehouseName").value;
    var date = document.getElementById("datePicker").value;
    dataTable.destroy();
    loadDataTable(x, date);

});



function loadDataTable(id,date) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/FruitCustomer/Slots/GetAll?id=" +id + "&date=" + date
        },
        "columns": [
            {
                "data": "date",
                'render': function (jsonDate) {

                    var datestr = jsonDate.toString();
                    var date = new Date(datestr.substr(0, 10));
                    var month = ("0" + (date.getMonth() + 1)).slice(-2);
                    return ("0" + date.getDate()).slice(-2) + '-' + month + '-' + date.getFullYear();
                    //return date;
                }, "width": "15%"
            },
            { "data": "warehouse.name", "width": "15%" },
            {
                "data": "startTime",
                'render': function (time) {


                    return time + " : 00";
                    //return date;
                },
                "width": "15%"
            },
            {
                "data": "endTime",
                'render': function (time) {


                    return time + " : 00";
                    //return date;
                },
                "width": "15%"
            },

            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/FruitCustomer/Slots/Edit/${data}" class="btn btn-success text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="Edit Closed Slot Time">
                                    <i class="fas fa-edit"></i> 
                                </a>
                               
                                <a href="/FruitCustomer/Slots/Details/${data}" class="btn btn-primary text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="View Closed Slot Time Details">
                                    <i class="fas fa-info"></i> 
                                </a>
                                <a onclick=Delete("/FruitCustomer/Slots/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="Delete Closed Slot Time">
                                    <i class="fas fa-trash-alt"></i> 
                                </a>
                            </div>
                           `;
                }, "width": "40%"
            }
        ]
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