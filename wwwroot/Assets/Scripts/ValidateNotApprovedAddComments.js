$(document).ready(function () {
   $("#placeholder").hide();
    $('#btnNotApprove').on('click', function (e) {

        var comments = $("#comments").val();

        if (comments === '' || comments === undefined) {
            // validation code here
            $("#placeholder").empty();
            $("#placeholder").hide();
            $("#placeholder").append("<div class='row'>" +
                "<div class='alert alert-danger col md-12'>" +
                "<strong>Comments are required when not approving.</strong>" +
                "</div></div>");
            e.preventDefault();
            $("#placeholder").show();
        }   
        else {
            $('form').unbind('submit').submit();
        }
    });
});

