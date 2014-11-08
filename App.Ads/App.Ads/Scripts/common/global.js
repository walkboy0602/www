
(function ($) {
    $.validator.unobtrusive.adapters.addBool("booleanrequired", "required");
}(jQuery));


//Handling broken image
function imgError(image, width, height) {
    image.onerror = "";
    size = width + "x" + height;
    image.src = "holder.js/" + size + "/text-NO IMAGE AVAILABLE";
    Holder.add_image("", image.src).run();
    return true;
}

//Get QueryString
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

$(document).ready(function () {
    //fancybox
    $(".fancybox").fancybox({
        prevEffect: 'elastic',
        nextEffect: 'elastic',
        closeBtn: true,
        autoSize: false,
        maxWidth: 800,
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

    $('img.lazy').each(function () {
        $(this).loadImage();
    })

    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });

});

