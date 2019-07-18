// ----==== For responsive google charts ====------------------------------------------------------

$(window).resize(function () {

	// Only call the function if it's defined (might be SLOW), which prevents errors in logging.

	if (typeof drawCharts == 'function') {
		drawCharts();
	}
});

// ----==== For Bootstrap ====---------------------------------------------------------------------

// Uses JQuery to attach a function to load and resize events.

$(window).on("load resize", function () {
	// Change the height style attribute of the class .fill-screen to the window's inner height.

	$(".fill-screen").css("height", window.innerHeight);

	var screenRelative = $(".fill-screenrelative");

//	var location = elementLocation(screenRelative);

	var top = screenRelative.offset().top;
	var margin = 5;

	$(".fill-screenrelative").css("height", window.innerHeight - top - margin);
});

$(document).ready(function () {
	// Change any element whose data-toggle attribute equals "tooltip" to execute the tooltip() method.

	$('[data-toggle="tooltip"]').tooltip();
});

// ----==== For Twitter ====-----------------------------------------------------------------------
// For Twitter "follow" and "tweet" buttons.

!function (d, s, id) {

	var js, fjs = d.getElementsByTagName(s)[0], p = /^http:/.test(d.location) ? 'http' : 'https';

	if (!d.getElementById(id)) {
		js = d.createElement(s);
		js.id = id;
		js.src = p + '://platform.twitter.com/widgets.js';
		fjs.parentNode.insertBefore(js, fjs);
	}

}(document, 'script', 'twitter-wjs');