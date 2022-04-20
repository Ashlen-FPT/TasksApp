
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
});

function RenderActions(RenderActionstring) {
    $("#OpenDialog").load(RenderActionstring);
};

function CreateNew() {
    if (!ValidateInputs())
        return;

    $.ajax({
        url: '/TaskRef/Create/',
        type: 'POST',
        data: $('form').serialize(),
        success: function (response) {
            Clean();
            $('#btnCloseModal').click();
            var raw = '';
            raw += "<tr id=" + response.Id + '>';
            raw += "<td>" + response.Id + "</td>";
            raw += "<td>" + response.Description + "</td>";
            raw += "<td>" + response.IsDone + "</td>";
            raw += "<td>" + response.Date + "</td>";
            raw += "<td>" + response.Comment + "</td>";
            raw += "<td>" + "<button class = \"btn btn-sm btn-primary\" data-toggle=\"modal\" data-target=\"#modalCreate\" onclick=\"RenderActions('/TaskRef/Edit/' + " + response.Id + ")\">Edit</button> | " +
                "<button class = \"btn btn-sm btn-danger\" data-toggle=\"modal\" data-target=\"#modalCreate\" onclick=\"RenderActions('/TaskRef/Delete/' + " + response.Id + ")\">Delete</button></td>";
            raw += "</tr>";
            $('#indexTbody').append(raw);
        },
        error: function (err) { alert("Error: " + err.responseText); }
    })
};

function DeleteEmp(id) {
    document.getElementById(id).remove();
    $.ajax({
        url: '/TaskRef/Delete/' + id,
        data: $('form').serialize(),
        type: 'POST',
        success: function () { $('#btnCloseModal').click(); },
        error: function (err) { alert("Error: " + err.responseText); }
    });
};

function EditEmp(id) {
    if (!ValidateInputs())
        return;
    $.ajax({
        url: '/TaskRef/Edit/' + id,
        type: 'POST',
        data: $('form').serialize(),
        success: function (res) {
            var keys = ["ID", "Description", "IsDone", "Date", "Comment"];
            $('#' + res.Id + ' td').each(function (i) {
                $(this).text(res[keys[i]]);
            });
            $('#btnCloseModal').click();
        },
        error: function (err) { alert("Error: " + err.responseText); }
    })
};

function Clean() {
    $('#modalCreate').find('textarea,input').val('');
};

function ValidateInputs() {
    var flag = true;
    var descriptionInput = $('#Description');
    var IsDoneInput = $('#IsDone');
    var DateInput = $('#Date');

    if ($.trim(descriptionInput.val()) != '') {
        descriptionInput.closest('.form-group').removeClass('has-error');
        flag = true;
    }

    if ($.trim(IsDoneInput.val()) != '') {
        IsDoneInput.closest('.form-group').removeClass('has-error');
        flag = true;
    }

    if ($.trim(emailInput.val()) === '') {
        emailInput.closest('.form-group').removeClass('has-error');
        flag = true;
    }

    if ($.trim(descriptionInput.val()) === '') {
        descriptionInput.closest('.form-group').addClass('has-error');
        flag = false;
    }

    if ($.trim(IsDoneInput.val()) === '') {
        IsDoneInput.closest('.form-group').addClass('has-error');
        flag = false;
    }

    if ($.trim(DateInput.val()) != '') {
        var reg = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        if (!reg.test($('#Email').val())) {
            DateInput.closest('.form-group').addClass('has-error');
            flag = false;
        }
    }

    return flag;
};
