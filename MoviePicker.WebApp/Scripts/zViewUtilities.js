var consoleOn = false;

// Formats a number with commas and truncates the decimal.
function formatWithCommas(value) {
	var valueAsString = value.toString();
	var index = valueAsString.indexOf('.');

	if (index >= 0) {
		valueAsString = valueAsString.substring(0, index);		// left
	}

	return valueAsString.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
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