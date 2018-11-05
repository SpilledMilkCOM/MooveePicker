var MINER_COUNT = 2;
var SCREEN_COUNT_MAX = 8;

function boxOfficeLostFocus(oldValue, newValue, movieIndex) {

	if (oldValue != newValue) {
		global().lastMovieControlIndex = movieIndex;

		clearWeights();

		clearMoviePicksPosters('bonusOnMovieList');
		clearMoviePicksPosters('bonusOffMovieList');

		updateSlider(newValue, movieIndex);

		updateBoxOffice(movieIndex);

		clickPicksAsync();
	}
}

function clearMoviePicksPosters(movieListId) {

	for (var movieCount = 0; movieCount < SCREEN_COUNT_MAX; movieCount++) {
		var image = $('#' + movieListId + 'MoviePosterId0' + movieCount);

		if (image != null) {
			//console.log(image.attr('src'));

			image.attr('src', '/Images/MooveePosterRecalculate.jpg');
			//image.attr('style', 'border-radius: 3px; box-shadow: 2px 4px 8px 0px grey;');

			// Change only the box-shadow.  Leave everything else alone!
			image.css({ 'box-shadow': '2px 4px 8px 0px grey' });
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

	navigateTo('/Home/Index');
}

function clickPicksAsync() {
	logit("clickPicksAsync");

	var parameters = parseBoxOfficeAndWeights();

	logit(parameters);

	var url = "/Home/Calculate?" + parameters;

	var baseUrl = parseBaseUrl();

	logit(baseUrl + url);

	calculate(url);
}

function clickTracking(uniqueId) {

	logit('clickTracking');

	// The unique ID is needed so Twitter won't cache the page/image

	navigateTo('/Home/Tracking', 'id=' + uniqueId);
}

// Navigate to the relative URL using the parsed BO and Weights as Request parameters.
function navigateTo(relativeUrl, moreParameters) {

	logit('navigateTo');

	var parameters = parseBoxOfficeAndWeights();

	logit(parameters);

	var url = relativeUrl + '?' + parameters;

	if (moreParameters != null) {
		url += '&' + moreParameters;
	}

	var baseUrl = parseBaseUrl();

	logit(baseUrl + url);

	window.location.href = baseUrl + url;

	logit(window.location.href);
}

// Navigate to the relative URL. (DON'T add BO and Weights to the request)
function navigateToExplicit(relativeUrl) {

	logit('navigateToExplicit');

	var baseUrl = parseBaseUrl();

	logit(baseUrl + relativeUrl);

	window.location.href = baseUrl + relativeUrl;

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

// Convert a percentage between -100 and 100 from (-red- to white to +green+)
function percentToBackgroundColor(percent) {
	var greenHue = 115;
	var redHue = 0;
	var hue = percent >= 0 ? greenHue : redHue;
	var minLuminosity = 32;			// 0 - 100%
	var luminosity = 0;
	var saturation = 100;			// 0 - 100%

	percent = Math.abs(percent);

	luminosity = (100 - percent) / 100 * (100 - minLuminosity) + minLuminosity;

	return 'hsl(' + hue + ', ' + saturation + '%, ' + luminosity + '%)';
}

function sliderOnChange(slider, controlIndex) {
	logit(controlIndex + ' - ' + slider.value);

	global().lastMovieControlIndex = controlIndex;

	clearWeights();

	var boxOffice = $('#boId' + controlIndex);
	var boxOfficePct = $('#boId' + controlIndex + 'Pct');

	if (boxOfficePct != null && boxOffice != null) {
		var originalValue = boxOffice.attr('data-original-value').replace(/,/g, '');

		logit(originalValue);
		logit(slider.value);
		logit(percentToBackgroundColor(slider.value));

		//boxOfficePct.attr('style', 'background-color: ' + percentToBackgroundColor(slider.value) + '; border-radius: 3px; padding: 3px;');
		boxOfficePct.css({ 'background-color': percentToBackgroundColor(slider.value) });
		boxOfficePct.text(slider.value + '%');
		boxOffice.val(formatWithCommas(originalValue * (100.0 + parseInt(slider.value)) / 100.0));

		clickPicksAsync();
	}
}

// Reformats the box office field with commas
function updateBoxOffice(controlIndex) {
	var boxOffice = $('#boId' + controlIndex);

	if (boxOffice != null) {
		boxOffice.val(formatWithCommas(boxOffice.val().replace(/\,/g, "")));
	}
}

function updateSlider(newValue, controlIndex) {
	logit('updateSlider');

	var boSlider = $('#boSliderId' + controlIndex);
	var boxOffice = $('#boId' + controlIndex);
	var boxOfficePct = $('#boId' + controlIndex + 'Pct');

	logit(boSlider);
	logit(controlIndex + ' -- ' + newValue.replace(/\,/g, ""));

	if (boSlider != null && boxOffice != null && boxOfficePct != null) {
		var originalValue = boxOffice.attr('data-original-value').replace(/,/g, '');

		logit(originalValue);
		logit((newValue.replace(/\,/g, "") - originalValue) / originalValue * 100);

		boSlider.val((newValue.replace(/\,/g, "") - originalValue) / originalValue * 100);
		boxOfficePct.text(boSlider.val() + '%');
	}
}

// Calculate ASYNC if the new value is different from the original value.
function weightLostFocus(newValue, controlIndex) {

	var weightField = $('#weightId' + controlIndex);
	var originalValue = 0;

	if (weightField != null) {
		originalValue = weightField.attr('data-original-value');
	}

	if (originalValue != newValue) {
		console.log('weightLostFocus');

		clearMoviePicksPosters('bonusOnMovieList');
		clearMoviePicksPosters('bonusOffMovieList');

		clickPicksAsync();

		var weightField = $('#weightId' + controlIndex);

		if (weightField != null) {
			weightField.attr('data-original-value', newValue);
		}
	}
}

function changeAttribute(id, newValue) {
}