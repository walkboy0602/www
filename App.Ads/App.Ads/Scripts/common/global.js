
(function ($) {
    $.validator.unobtrusive.adapters.addBool("booleanrequired", "required");
}(jQuery));

$(document).ready(function () {

    //$(".fancybox").fancybox();

    //fancybox
    $(".fancybox").fancybox({
        prevEffect: 'none',
        nextEffect: 'none',
        closeBtn: false,
        autoSize: false,
        maxWidth: 680,
        maxHeight: 480,
        helpers: {
            //float, outside, inside, over
            title: { type: 'float' },
            buttons: {},
            //thumbs: {
            //    width: 50,
            //    height: 50
            //}
        }
    });

    $(".fancybox-button").fancybox({
        prevEffect: 'none',
        nextEffect: 'none',
        closeBtn: false,
        helpers: {
            title: { type: 'inside' },
            buttons: {}
        }
    });

    $('.fancybox-media').fancybox({
        openEffect: 'none',
        closeEffect: 'none',
        helpers: {
            media: {}
        }
    });


});