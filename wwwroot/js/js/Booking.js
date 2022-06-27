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


function loadDataTable(WarehouseId, date) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/FruitCustomer/Booking/GetAllByWarehouseDate?id=" + WarehouseId + "&date=" + date
        }, dom: 'Bfrtip',
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
            { "data": "bookingRef", "width": "5%" },
            {
                "data": "date",
                'render': function (jsonDate) {

                    var datestr = jsonDate.toString();
                    var date = new Date(datestr.substr(0, 10));
                    var month = ("0" + (date.getMonth() + 1)).slice(-2);
                    return ("0" + date.getDate()).slice(-2) + '-' + month + '-' + date.getFullYear();
                    //return date;
                }, "width": "5%"
            },
            {
                "data": "rowIndex",
                'render': function (time) {


                    return time + " : 00";
                    //return date;
                },
                "width": "5%"
            },
            { "data": "transporter.name", "width": "5%" },
            { "data": "registration", "width": "5%" },
            { "data": "exporter.name", "width": "5%" },
            { "data": "numOfPlts", "width": "5%" },
            { "data": "phoneNumber", "width": "5%" },
            { "data": "email", "width": "5%" },
            {
                "data": "status",

                'render': function (status) {

                    if (status == "Truckstop Arrival") {
                        return "At the Truck Stop";
                    }

                    if (status == "Truckstop Depature") {
                        return "En route to Warehouse";
                    }

                    if (status == "Gate In") {
                        return "At the warehouse";
                    }

                    if (status == "Gate Out") {
                        return "Complete";
                    }

                    else {
                        return "BOOKED";
                    }


                    //return date;
                }

                , "width": "5%"
            },
            {
                "data": "createdDateUtc",
                'render': function (jsonDate) {

                    var datestr = jsonDate.toString();
                    var date = new Date(datestr.substr(0, 10));
                    var month = ("0" + (date.getMonth() + 1)).slice(-2);

                    var time = datestr.substr(11)

                    var hr = time.substr(0, 2)
                    var min = time.substr(3, 2)
                    var sec = time.substr(6, 2);

                    return ("0" + date.getDate()).slice(-2) + '-' + month + '-' + date.getFullYear() + " " + hr + ":" + min + ":" + sec;
                    //return date;
                }, "width": "5%"
            },
            {
                "data": {
                    id: "id", status: "status"
                },
                "render": function (data) {

                    if (data.status == "BOOKED") {
                        return `
                                                    <div class="text-center">
                                                        <a href="/FruitCustomer/Booking/Edit/${data.id}" class="btn btn-success text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="Edit Booking">
                                                            <i class="fas fa-edit"></i> 
                                                        </a>

                                                        <a href="/FruitCustomer/Booking/Details/${data.id}" class="btn btn-primary text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="View Details">
                                                            <i class="fas fa-info"></i> 
                                                        </a>

                                                        <a onclick=Delete("/FruitCustomer/Booking/Delete/${data.id}") class="btn btn-danger text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="Delete Booking">
                                                            <i class="fas fa-trash-alt"></i> 
                                                        </a>

                                                    </div>
                                                   `;
                    }

                    else {
                        return `
                                                    <div class="text-center">
                                                        <a href="/FruitCustomer/Booking/Edit/${data.id}" class="btn btn-success text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="Edit Booking">
                                                            <i class="fas fa-edit"></i> 
                                                        </a>

                                                        <a href="/FruitCustomer/Booking/Details/${data.id}" class="btn btn-primary text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="View Details">
                                                            <i class="fas fa-info"></i> 
                                                        </a>


                                                    </div>
                                                   `;
                    }

                }, "width": "8%"
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

