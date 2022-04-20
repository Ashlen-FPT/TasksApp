var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/User/GetAll"
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
            { "data": "role", "width": "15%" }
        ]
    });
}



