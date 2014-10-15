(function($) {
    $(document).ready(function() {
        $("input[type=text], input[type=password]")
             .focus(function() {
                 $(this).addClass("active");
             }).blur(function() {
                 $(this).removeClass("active");
             });
    });
})(jQuery);

; (function($) {

    var loadingTimer, loadingFrame = 1;

    var loading2;

    $.loading = function() {

    };

    $.loading.showActivity = function() {
        clearInterval(loadingTimer);

        loading2.show();

        loadingTimer = setInterval(loading2_animate_loading, 66);
    };

    $.loading.hideActivity = function() {
        loading2.hide();
    };

    function loading2_animate_loading() {
        if (!loading2.is(':visible')) {
            clearInterval(loadingTimer);
            return;
        }

        $('div', loading2).css('top', (loadingFrame * -40) + 'px');

        loadingFrame = (loadingFrame + 1) % 12;
    };

    $(document).ready(function() {
        $('body').append(
			        loading2 = $('<div id="loading2"><div></div></div>')
		        );
		        
        $.loading.showActivity();
    });

})(jQuery);


$(document).ready(function() {
    $('a.popup').live('click', function() {
        $.fancybox({
            'padding': 0,
            'autoScale': false,
            'transitionIn': 'none',
            'transitionOut': 'none',
            'title': this.title,
            'centerOnScroll': true,
            'width': 680,
            'height': 495,
            'href': $(this).attr('href'),
            onComplete: function() {
                // $('#abuseform #name').focus();
            },
            onCleanup: function() {
                // $('#abuseform').validate().resetForm();
            }
        });
        return (false);
    });
});
