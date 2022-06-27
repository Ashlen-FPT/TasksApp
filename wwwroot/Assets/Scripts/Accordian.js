    $(document).ready(function () {

        //ACCORDION BUTTON ACTION
        $('div.accordionButton').click(function () {
            $('div.accordionContent').slideUp('normal');
            //$(this).children().eq(1).addClass('icon-arrow-up').removeClass('icon-arrow-down');
            if (!$(this).next().is(':visible')) {
                //$(this).children().eq(1).addClass('icon-arrow-down').removeClass('icon-arrow-up');
                $(this).next().slideDown('normal');
            }
        });
        $("div.accordionContent").hide();

        $('div.accordionButton1').click(function () {
            $('div.accordionContent1').slideUp('normal');
            //$(this).children().eq(1).addClass('icon-arrow-up').removeClass('icon-arrow-down');
            if (!$(this).next().is(':visible')) {
                //$(this).children().eq(1).addClass('icon-arrow-down').removeClass('icon-arrow-up');
                $(this).next().slideDown('normal');
            }
        });

        //HIDE THE DIVS ON PAGE LOAD
        $("div.accordionContent1").hide();
    });