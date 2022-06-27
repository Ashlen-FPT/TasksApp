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
        },
        "columns": [
            { "data": "bookingRef", "width": "10%" },
            {
                "data": "date",
                'render': function (jsonDate) {

                    var datestr = jsonDate.toString();
                    var date = new Date(datestr.substr(0, 10));
                    var month = ("0" + (date.getMonth() + 1)).slice(-2);
                    return ("0" + date.getDate()).slice(-2) + '-' + month + '-' + date.getFullYear();
                    //return date;
                }, "width": "10%"
            },
            {
                "data": "rowIndex",
                'render': function (time) {


                    return time + " : 00";
                    //return date;
                },
                "width": "5%"
            },
            { "data": "transporter.name", "width": "10%" },
            { "data": "registration", "width": "10%" },
            { "data": "exporter.name", "width": "10%" },
            { "data": "numOfPlts", "width": "10%" },
            { "data": "email", "width": "5%" },
            
            { "data": "status", "width": "10%" },
            {
                "data": {
                    id: "id", status: "status"
                },
                "render": function (data) {

                    if (data.status == "Truckstop Arrival") {
                        return `
                            <div class="text-center">
                                <a onclick=changeStatusTBRN(${data.id}) id="addTBRN"  class="btn btn-danger btn-lg text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="Change Status" >
                                <i class="fas fa-level-up-alt"></i>
                                </a>

                                <a href="/FruitCustomer/Booking/Details/${data.id}" class="btn btn-primary btn-lg text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="View Details">
                                    <i class="fas fa-info"></i> 
                                </a>
                               
                            </div>
                           `;
                    }
                    else {
                        return `
                            <div class="text-center">
                                <a onclick=changeStatus("/FruitCustomer/Booking/ChangeStatus/${data.id}") class="btn btn-warning btn-lg text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="Change Status" >
                                   <i class="fas fa-level-up-alt"></i>
                                </a>                        

                                <a href="/FruitCustomer/Booking/Details/${data.id}" class="btn btn-primary btn-lg text-white" style="cursor:pointer" data-toggle="tooltip" data-placement="top" title="View Details">
                                    <i class="fas fa-info"></i> 
                                </a>
                               
                            </div>
                           `;
                    }

                   
                }, "width": "25%"
            }
        ]
    });
}




function changeStatus(url) {

    var uri = url;
    var res = encodeURI(uri);


    swal({
        title: "Are you sure you want to change the Status of this Booking?",
        text: "You will not be able to undo this change!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: res,
                success: function (response) {
                    console.log(response)
                    toastr.success("Status Changed!");
                    dataTable.ajax.reload();

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    toastr.error("Error! PLease try again!");
                    dataTable.ajax.reload();
                }
            });
        }
    });
}


function InsertTBRN() {

    var TBRN = document.getElementById("TBRN").value;


    swal({
        title: "Confirm TBRN",
        text: "Please confirm that the TBRN is correct " + TBRN,
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: "/FruitCustomer/Booking/ChangeStatusTBRN/?id=" + id + "&TBRN=" + TBRN,
                success: function (response) {
                    toastr.success("Status Changed!");
                    dataTable.ajax.reload();

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    toastr.error("Error! PLease try again!");
                    dataTable.ajax.reload();
                }
            });
        }
    });

    $("#TBRNModal").modal('toggle');


}


function changeStatusTBRN(url) {
    $("#TBRNModal").modal('toggle');

    id = url;

}