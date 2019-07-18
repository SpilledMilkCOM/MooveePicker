var consoleOn = false;

// Change an element's style, given the id
function changeStyle(id, style, value) {
	var ctrl = $('#' + id);

	if (ctrl != null) {
		// You can do multiple styles with the method below.
		//ctrl.css({ style: value });

		// This sets one style attribute to a value.
		ctrl.css(style, value);
	}
}

// Change an element's text.
function changeText(id, value) {
	var ctrl = $('#' + id);

	if (ctrl != null && ctrl.text() != value) {
		ctrl.text(value);
	}
}

// Determine location of a DOM element.
function elementLocation(el) {
	var elementX = 0;
	var elementY = 0;

	// Add up all of the offsets from the element to the parent and older generations.

	for (; el != null; el = el.offsetParent) {
		elementX += el.offsetLeft;
		elementY += el.offsetTop;
	}

	return { x: elementX, y: elementY };
}

// Formats a number with commas and truncates the decimal.
function formatWithCommas(value) {
	var valueAsString = value.toString();
	var index = valueAsString.indexOf('.');

	if (index >= 0) {
		valueAsString = valueAsString.substring(0, index);		// left
	}

	return valueAsString.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

// Test for jQuery control existance
function controlExists(control) {
	return control != null && control.length;
}

// Converts text with commas into a raw value
function getRawValue(value) {
	return value.replace(/,/g, '');
}

// Log a message to the console (based on consoleOn flag set above)
function logit(message) {
	if (consoleOn == true) {
		console.log(message);
	}
}

// Sets the text of a give control id
function setText(id, value) {
	var ctrl = $('#' + id);

	if (ctrl != null) {
		ctrl.text(value);
	}
}