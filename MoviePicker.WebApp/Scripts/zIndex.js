
var MINER_COUNT = 2;
var SCREEN_COUNT_MAX = 8;
var inCallback = false;		// Prevent multiple retrievals of the same data recursion

function boxOfficeLostFocus(oldValue, newValue) {

	if (oldValue != newValue) {
		clearWeights();

		clearMoviePicksPosters('bonusOnMovieList');
		clearMoviePicksPosters('bonusOffMovieList');
	}
}

function clearMoviePicksPosters(movieListId) {

	for (var movieCount = 0; movieCount < SCREEN_COUNT_MAX; movieCount++) {
		var image = $('#' + movieListId + 'moviePosterId0' + movieCount);

		if (image != null) {
			//console.log(image.attr('src'));

			image.attr('src', '/Images/MooveePosterRecalculate.jpg');
		}
	}
}

function clearWeights() {
	//console.log("clearWeights");

	for (idx = MINER_COUNT; idx <= 7; idx++) {
		$("#weightId" + idx).val(0);

		//console.log("#weightId" + idx + " = " + $("#weightId" + idx).val());
	}
}

function clickPasteBoxOffice() {
	var boValues = $("#pasteAreaId").val();
	var splitValues = boValues.split("\n");

	for (idx = 0; idx < 15 && idx < splitValues.length; idx++) {

		//console.log(idx + " = " + splitValues[idx]);

		if (idx < splitValues.length) {
			$("#boId" + (idx + 1)).val(splitValues[idx].trim());
		}
		else {
			$("#boId" + (idx + 1)).val("0");
		}
	}

	clearWeights();

	var parameters = parseBoxOfficeAndWeights();

	console.log(parameters);

	clickPicks();
}

function clickPicks() {

	console.log("clickPicks");

	var parameters = parseBoxOfficeAndWeights();

	console.log(parameters);

	var url = "/Home/Index2?" + parameters;

	var baseUrl = parseBaseUrl();

	console.log(baseUrl + url);

	window.location.href = baseUrl + url;

	console.log(window.location.href);
}

function clickPicks2() {
	console.log("clickPicks");

	var parameters = parseBoxOfficeAndWeights();

	console.log(parameters);

	var url = "/Home/Index2?" + parameters;

	var baseUrl = parseBaseUrl();

	console.log(baseUrl + url);
}

function clickTracking() {

	var parameters = parseBoxOfficeAndWeights();

	var url = "/Home/Tracking?" + parameters;

	var baseUrl = parseBaseUrl();

	window.location.href = baseUrl + url;
}

function parseBaseUrl() {
	var baseUrl = window.location.href;

	if (baseUrl != null) {
		var slashIdx = baseUrl.indexOf("/", "https://".length);

		if (slashIdx > 0) {
			baseUrl = baseUrl.substring(0, slashIdx);
		}
	}

	return baseUrl;
}

function parseBoxOfficeAndWeights() {

	var boList = "";
	var weightList = "";
	var weightTotal = 0;

	// The values should be synchronized so the values can be pulled from one set of fields.

	for (idx = 1; idx <= 15; idx++) {
		if (idx != 1) {
			boList += ",";
		}

		// Replace all commas (global g parameter on regular expression)
		boList += $("#boId" + idx).val().replace(/\,/g, "").replace("$", "");

		console.log(boList);
	}

	for (idx = MINER_COUNT; idx <= 7; idx++) {
		if (idx != MINER_COUNT) {
			weightList += ",";
		}

		// TODO: Request encode this.

		var weight = $("#weightId" + idx).val().replace(",", "").replace("$", "");

		weightList += weight;
		weightTotal += weight;

		console.log(weightList);
		console.log(weightTotal);
	}

	if (weightTotal == 0) {
		weightList = "1," + weightList;
	}
	else {
		weightList = "0," + weightList;
	}

	console.log(boList);

	return "bo=" + boList + "&wl=" + weightList;
}

function weightLostFocus(oldValue, newValue) {

	if (oldValue != newValue) {
		clearMoviePicksPosters('bonusOnMovieList');
		clearMoviePicksPosters('bonusOffMovieList');

		clickPicks();
	}
}