var MINER_COUNT = 2;
var SCREEN_COUNT_MAX = 8;

function boxOfficeCompoundLostFocus(oldValue, newValue) {

	logit('boxOfficeCompoundLostFocus()');

	if (oldValue != newValue) {

		clearWeights();

		clearMoviePicksPosters('bonusOnMovieList');
		clearMoviePicksPosters('bonusOffMovieList');

		var compoundField = $('#compoundFieldId');

		if (controlExists(compoundField)) {
			updateCompoundEqualPercentages();

			// Change the compound movies based on their percentage

			updateCompoundBoxOffice(1, newValue);
			updateCompoundBoxOffice(2, newValue);
			updateCompoundBoxOffice(3, newValue);
		}

		clickPicksAsync();
	}
}

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

function clearMoviePicksPostersAll() {
	clearMoviePicksPosters('bonusOnMovieList');
	clearMoviePicksPosters('bonusOffMovieList');
}

function clearWeights() {
	logit("clearWeights()");

	for (idx = MINER_COUNT; idx <= 7; idx++) {
		$("#weightId" + idx).val(0);

		logit("#weightId" + idx + " = " + $("#weightId" + idx).val());
	}
}

function clickChangeCompoundPercent(change) {
	var compoundField = $('#compoundFieldId');

	if (controlExists(compoundField)) {
		var oldValue = compoundField.val().replace(/,/g, '');
		var newValue = oldValue * (100.0 + change) / 100.0;

		compoundField.val(formatWithCommas(newValue));

		boxOfficeCompoundLostFocus(oldValue, formatWithCommas(newValue));		// "generate" the event and trigger the change.
	}
}

function clickChangePercent(button, controlIndex, change) {
	logit('clickChangePercent() - ' + controlIndex + ' - changing percent ' + change);

	global().lastMovieControlIndex = controlIndex;

	clearWeights();

	var boxOffice = $('#boId' + controlIndex);
	var boxOfficePct = $('#boId' + controlIndex + 'Pct');

	if (boxOfficePct != null && boxOffice != null) {
		logit('boxOfficePct.text() = ' + boxOfficePct.text());

		var originalValue = boxOffice.attr('data-original-value').replace(/,/g, '');
		var newPctValue = parseFloat(boxOfficePct.text().replace(/%/g, '').replace(/,/g, '')) + change;

		if (newPctValue < -100) {
			newPctValue = -100;
		}

		logit('originalValue = ' + originalValue);
		logit('newPctValue = ' + newPctValue);
		logit(percentToBackgroundColor(newPctValue));

		if (originalValue <= 0) {
			originalValue = 1;
		}

		boxOfficePct.css({ 'background-color': percentToBackgroundColor(newPctValue) });
		boxOfficePct.text(formatWithCommas(newPctValue) + '%');
		boxOffice.val(formatWithCommas(originalValue * (100.0 + newPctValue) / 100.0));

		logit('boxOffice.val() = ' + boxOffice.val());

		clickPicksAsync();
	}
}

function clickCollapseTitle() {
	var titleSection = $('#titleSection');

	if (controlExists(titleSection)) {
		titleSection.css({ 'display': 'none' });
	}

	var minerWeights = $('#minerWeights');

	if (controlExists(minerWeights)) {
		minerWeights.css({ 'display': 'none' });
	}

	var showTitle = $('#showTitle');

	if (controlExists(showTitle)) {
		showTitle.css({ 'display': 'initial' });
	}

	//window.onresize();
	window.dispatchEvent(new Event('resize'));
}

function clickHidePoster(posterId) {

	// Also need functionality to move the other DIVs up into this postion.

	logit('clickHidePoster(' + posterId + ')');

	var div = $('#' + posterId);

	if (div != null) {
		div.css({ 'display': 'none' });
	}
}

function clickMorePicks() {

	logit('clickMorePicks');

	navigateTo('/Home/MorePicks');
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

function clickShowTitle() {
	var titleSection = $('#titleSection');

	if (controlExists(titleSection)) {
		titleSection.css({ 'display': 'initial' });
	}

	var minerWeights = $('#minerWeights');

	if (controlExists(minerWeights)) {
		minerWeights.css({ 'display': 'initial' });
	}

	var showTitle = $('#showTitle');

	if (controlExists(showTitle)) {
		showTitle.css({ 'display': 'none' });
	}

	window.dispatchEvent(new Event('resize'));
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

// Return the base url (root domain) from the window's location HREF
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

// Aggregate the Box Office and Weight fields into Request arguments.
function parseBoxOfficeAndWeights() {

	logit("parseBoxOfficeAndWeights");

	logit($("#boId1").length);

	if ($("#boId1").length === 0) {
		// Nothing to parse on this page.
		return "";
	}

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

		var weightField = $("#weightId" + idx);
		var weight = 0;

		logit(weightField);

		if (weightField != null && weightField.val() != null) {
			// The field might be hidden so check for it first.

			weight = weightField.val().replace(",", "").replace("$", "");
		}

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

	if (luminosity < minLuminosity) {
		luminosity = minLuminosity;
	}

	return 'hsl(' + hue + ', ' + saturation + '%, ' + luminosity + '%)';
}

// Callback for range/slider control
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

		boxOfficePct.css({ 'background-color': percentToBackgroundColor(slider.value) });
		boxOfficePct.text(formatWithCommas(slider.value) + '%');
		boxOffice.val(formatWithCommas(originalValue * (100.0 + parseInt(slider.value)) / 100.0));

		clickPicksAsync();
	}
}

// Reformats the box office field with commas
function updateBoxOffice(controlIndex) {
	var boxOffice = $('#boId' + controlIndex);

	if (controlExists(boxOffice)) {
		var value = boxOffice.val().replace(/\,/g, "");
		//var isNew = boxOffice.attr('data-isnew') == "true";

		//if (isNew) {
		//	// Since this is called from LostFocus, THIS value should be set into the orginal value.
		//	// Only the scroll buttons will adjust the percentage (leaving original-value alone).

		//	boxOffice.prop('data-original-value', value);
		//}

		boxOffice.val(formatWithCommas(value));
	}
}

function updateCompoundBoxOffice(controlId, newValue) {

	logit('updateCompoundBoxOffice(' + controlId + ', ' + newValue + ')');

	newValue = newValue.replace(/,/g, '');

	var boxOfficeCompoundPct = $('#boId' + controlId + 'CompoundPct');

	if (controlExists(boxOfficeCompoundPct)) {
		var pctValue = parseFloat(boxOfficeCompoundPct.text().replace(/%/g, '').replace(/,/g, ''));

		logit('pctValue = ' + pctValue);

		// Muliply new compound total by percentages to change each value.
		// Don't need to change the percent.

		var boxOffice = $('#boId' + controlId);
		var value = newValue * pctValue / 100.0;

		boxOffice.val(formatWithCommas(value));
		//boxOffice.prop('data-original-value', value);
		boxOffice.attr('data-original-value', value);

		logit('data-original-value = ' + boxOffice.attr('data-original-value'));
	}
}

// Adjust the percentages if they are all 0, make them all 33.3%
function updateCompoundEqualPercentages() {
	logit('updateCompoundEqualPercentages()');

	var allEqual = false;
	var pctValue1 = 0;
	var pctValue2 = 0;

	var boxOfficeCompoundPct = $('#boId1CompoundPct');

	if (controlExists(boxOfficeCompoundPct)) {
		pctValue1 = parseInt(boxOfficeCompoundPct.text().replace(/%/g, '').replace(/,/g, ''));

		logit('pctValue1 = ' + pctValue1);

		if (pctValue1 == 0) {
			boxOfficeCompoundPct = $('#boId2CompoundPct');

			if (controlExists(boxOfficeCompoundPct)) {
				pctValue2 = parseInt(boxOfficeCompoundPct.text().replace(/%/g, '').replace(/,/g, ''));

				logit('pctValue2 = ' + pctValue2);

				allEqual = pctValue1 == pctValue2;

				if (allEqual) {
					boxOfficeCompoundPct = $('#boId3CompoundPct');

					if (controlExists(boxOfficeCompoundPct)) {
						pctValue2 = parseInt(boxOfficeCompoundPct.text().replace(/%/g, '').replace(/,/g, ''));

						logit('pctValue2 = ' + pctValue2);

						allEqual = pctValue1 == pctValue2;
					}
				}
			}
		}

	}

	if (allEqual) {
		boxOfficeCompoundPct = $('#boId1CompoundPct');

		if (controlExists(boxOfficeCompoundPct)) {
			boxOfficeCompoundPct.text('33.3%');
		}

		boxOfficeCompoundPct = $('#boId2CompoundPct');

		if (controlExists(boxOfficeCompoundPct)) {
			boxOfficeCompoundPct.text('33.3%');
		}

		boxOfficeCompoundPct = $('#boId3CompoundPct');

		if (controlExists(boxOfficeCompoundPct)) {
			boxOfficeCompoundPct.text('33.3%');
		}
	}

	logit('EXIT - updateCompoundEqualPercentages()');
}

// Update the slider when other values change.
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
		boxOfficePct.text(formatWithCommas(boSlider.val()) + '%');
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

		clearMoviePicksPostersAll();

		clickPicksAsync();

		var weightField = $('#weightId' + controlIndex);

		if (weightField != null) {
			weightField.attr('data-original-value', newValue);
		}
	}
}

function changeAttribute(id, newValue) {
}