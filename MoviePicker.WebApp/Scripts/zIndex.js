var MINER_COUNT = 2;
var SCREEN_COUNT_MAX = 8;

function boxOfficeLostFocus(oldValue, newValue, movieIndex) {

	if (oldValue != newValue) {
		clearWeights();

		clearMoviePicksPosters('bonusOnMovieList');
		clearMoviePicksPosters('bonusOffMovieList');

		updateSlider(newValue, movieIndex);

		clickPicks2();
	}
}

function clearMoviePicksPosters(movieListId) {

	for (var movieCount = 0; movieCount < SCREEN_COUNT_MAX; movieCount++) {
		var image = $('#' + movieListId + 'MoviePosterId0' + movieCount);

		if (image != null) {
			//console.log(image.attr('src'));

			image.attr('src', '/Images/MooveePosterRecalculate.jpg');
			image.attr('style', 'border-radius: 3px; box-shadow: 2px 4px 8px 0px grey;');
		}
	}
}

function clearWeights() {
	logit("clearWeights");

	for (idx = MINER_COUNT; idx <= 7; idx++) {
		$("#weightId" + idx).val(0);

		logit("#weightId" + idx + " = " + $("#weightId" + idx).val());
	}
}

function clickPasteBoxOffice() {
	var boValues = $("#pasteAreaId").val();
	var splitValues = boValues.split("\n");

	for (idx = 0; idx < 15 && idx < splitValues.length; idx++) {

		logit(idx + " = " + splitValues[idx]);

		if (idx < splitValues.length) {
			$("#boId" + (idx + 1)).val(splitValues[idx].trim());
		}
		else {
			$("#boId" + (idx + 1)).val("0");
		}
	}

	clearWeights();

	var parameters = parseBoxOfficeAndWeights();

	logit(parameters);

	clickPicks();
}

function clickMorePicks() {

	logit('clickMorePicks');

	navigateTo('/Home/MorePicks');
}

function clickPicks() {

	logit("clickPicks");

	navigateTo('/Home/Index2');
}

function clickPicks2() {
	logit("clickPicks2");

	var parameters = parseBoxOfficeAndWeights();

	logit(parameters);

	var url = "/Home/Calculate?" + parameters;

	var baseUrl = parseBaseUrl();

	logit(baseUrl + url);

	calculate(url);
}

function clickTracking() {

	logit("clickPicks");

	navigateTo('/Home/Tracking');
}

function navigateTo(relativeUrl) {

	logit("navigateTo");

	var parameters = parseBoxOfficeAndWeights();

	logit(parameters);

	var url = relativeUrl +'?' + parameters;

	var baseUrl = parseBaseUrl();

	logit(baseUrl + url);

	window.location.href = baseUrl + url;

	logit(window.location.href);
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

		logit(boList);
	}

	for (idx = MINER_COUNT; idx <= 7; idx++) {
		if (idx != MINER_COUNT) {
			weightList += ",";
		}

		// TODO: Request encode this.

		var weight = $("#weightId" + idx).val().replace(",", "").replace("$", "");

		weightList += weight;
		weightTotal += weight;

		logit(weightList);
		logit(weightTotal);
	}

	if (weightTotal == 0) {
		weightList = "1," + weightList;
	}
	else {
		weightList = "0," + weightList;
	}

	logit(boList);

	return "bo=" + boList + "&wl=" + weightList;
}

function sliderOnChange(slider, controlIndex) {
	logit(controlIndex + ' - ' + slider.value);

	clearWeights();

	var boxOffice = $('#boId' + controlIndex);
	var boxOfficePct = $('#boId' + controlIndex + 'Pct');

	if (boxOfficePct != null && boxOffice != null) {
		var originalValue = boxOffice.attr('data-original-value').replace(/,/g, '');

		logit(originalValue);
		logit(slider.value);

		boxOfficePct.text(slider.value + '%');
		boxOffice.val(formatWithCommas(originalValue * (100.0 + parseInt(slider.value)) / 100.0));

		clickPicks2();
	}
}

function updateSlider(newValue, controlIndex) {
	var boSlider = $('#boSliderId' + controlIndex);
	var boxOffice = $('#boId' + controlIndex);
	var boxOfficePct = $('#boId' + controlIndex + 'Pct');

	console.log(boSlider);
	console.log(controlIndex + ' -- ' + newValue.replace(/\,/g, ""));

	if (boSlider != null && boxOffice != null && boxOfficePct != null) {
		var originalValue = boxOffice.attr('data-original-value').replace(/,/g, '');

		console.log(originalValue);
		console.log((newValue.replace(/\,/g, "") - originalValue) / originalValue * 100);

		boSlider.val((newValue.replace(/\,/g, "") - originalValue) / originalValue * 100);
		boxOfficePct.text(boSlider.val() + '%');
	}
}

function weightLostFocus(oldValue, newValue) {

	if (oldValue != newValue) {
		console.log('weightLostFocus');

		clearMoviePicksPosters('bonusOnMovieList');
		clearMoviePicksPosters('bonusOffMovieList');

		clickPicks2();
	}
}