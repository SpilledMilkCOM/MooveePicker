// This contains all of the funtions to support the asynchronous calculate callback.

var inCallback = false;		// Prevent multiple retrievals of the same data recursion

// Fill in the already scaffolded Bonus Comparison section with new data.
function bonusComparison(movies) {

	for (var movieCount = 0, length = movies.length; movieCount < length; movieCount++) {
		var mostEfficient = movies[0];
		var movie = movies[movieCount];
		var nextMovie = null;

		// If the last box office edited matches the movie, then HIGHLIGHT it!
		// Otherwise, the top row is light green.
		// Otherwise, even rows are grey and odd rows are white (no color set).

		var backgroundColor = movie.ControlId == global().lastMovieControlIndex ? 'lemonchiffon' : (movieCount == 0 ? 'lightgreen' : (movieCount % 2 == 0 ? 'lightgrey' : 'white'));

		if (movieCount + 1 < length) {
			nextMovie = movies[movieCount + 1];
		}

		setText('bonusCompMovieName' + movieCount, movie.Name);
		setText('bonusCompMovieCost' + movieCount, movie.Cost);
		setText('bonusCompMovieEarnings' + movieCount, formatWithCommas(movie.Earnings));
		setText('bonusCompMovieEfficiency' + movieCount, (movie.Efficiency / 1000).toFixed(2));

		//console.log(mostEfficient);
		//console.log(nextMovie);

		if (movieCount == 0 && nextMovie != null) {
			//How much does the top rank have to LOSE before it's no longer the TOP
			setText('bonusCompDifference' + movieCount, formatWithCommas(nextMovie.Efficiency * movie.Cost - movie.Earnings));

			setText('bonusCompDiffPctDown' + movieCount, ((nextMovie.Efficiency * movie.Cost - movie.EarningsBase) / movie.EarningsBase * 100).toFixed(1) + '%');
		}
		else if (mostEfficient != null) {
			//How much does current rank to GAIN to be the TOP
			setText('bonusCompDifference' + movieCount, formatWithCommas(mostEfficient.Efficiency * movie.Cost - movie.EarningsBase));

			if (movie.EarningsBase > 0) {
				setText('bonusCompDiffPctUp' + movieCount, ((mostEfficient.Efficiency * movie.Cost - movie.EarningsBase) / movie.EarningsBase * 100).toFixed(1) + '%');
			}
			else {
				setText('bonusCompDiffPctUp' + movieCount, '--');
			}

			if (nextMovie != null && movie.EarningsBase > 0) {
				setText('bonusCompDiffPctDown' + movieCount, ((nextMovie.Efficiency * movie.Cost - movie.EarningsBase) / movie.EarningsBase * 100).toFixed(1) + '%');
			}
			else {
				setText('bonusCompDiffPctDown' + movieCount, '--');
			}
		}

		// NOTE: Not very efficient, because you have to retrieve the control using $('#' + id) above and below twice.

		changeStyle('bonusCompMovieName' + movieCount, 'background-color', backgroundColor);
		changeStyle('bonusCompMovieCost' + movieCount, 'background-color', backgroundColor);
		changeStyle('bonusCompMovieEarnings' + movieCount, 'background-color', backgroundColor);
		changeStyle('bonusCompMovieEfficiency' + movieCount, 'background-color', backgroundColor);
		changeStyle('bonusCompDifference' + movieCount, 'background-color', backgroundColor);
		changeStyle('bonusCompDiffPctUp' + movieCount, 'background-color', backgroundColor);
		changeStyle('bonusCompDiffPctDown' + movieCount, 'background-color', backgroundColor);
	}
}

// Fill in the already scaffoled Box Office fields and sliders.
function boxOffice(movies, bestPerformer) {

	for (var movieCount = 0, length = movies.length; movieCount < length; movieCount++) {
		var movie = movies[movieCount];
		var controlIndex = movie.ControlId;
		var boxOffice = $('#boId' + controlIndex);

		logit('controlIndex = ' + controlIndex);

		if (boxOffice != null) {
			var boxOfficeImage = $('#imageId' + controlIndex);
			var boxOfficePct = $('#boId' + controlIndex + 'Pct');
			var boxOfficeSlider = $('#boSliderId' + controlIndex);
			var originalValue = boxOffice.attr('data-original-value').replace(/,/g, '');
			var percent = (movie.EarningsBase - originalValue) / originalValue * 100;

			logit(originalValue);
			logit(percent + '%');

			boxOffice.val(formatWithCommas(movie.EarningsBase));

			if (boxOfficePct != null) {
				boxOfficePct.val(percent + '%');
			}

			boxOfficePct.css({ 'background-color': percentToBackgroundColor(percent) });

			if (boxOfficeSlider != null) {
				boxOfficeSlider.val(percent);
			}

			if (boxOfficeImage != null) {
				var shadowColor = "grey";

				if (bestPerformer != null && controlIndex == bestPerformer.ControlId) {
					shadowColor = "green";
				}

				boxOfficeImage.css({ 'box-shadow': '2px 4px 8px 0px ' + shadowColor});
			}
		}
	}
}

// ASYNC function to call the Home/Calculate endpoint and refresh the view with the new JSON data.
function calculate(endPoint) {

	if (inCallback == true) {
		// Bust out of the possible multiple calls to this while the callback is attempting to return data.
		return;
	}

	inCallback = true;

	logit("calculate('" + endPoint + "')");

	$.ajax({
		type: 'GET',
		url: endPoint,
		dataType: 'json',
		success: function (jsonData) {

			if (jsonData == null) {
				inCallback = false;
				logit('JSON data is null.');
				return;
			}

			// JSON data is a MovieList

			logit(jsonData);
			logit(jsonData.TotalEarnings);
			logit(jsonData.TotalCost);

			setText('durationId', formatWithCommas(jsonData.Duration));

			movieListMini(jsonData.MovieList.Picks[0], 'bonusOnMovieList', jsonData.MovieList.ShareQueryString);
			movieListMini(jsonData.MovieListBonusOff.Picks[0], 'bonusOffMovieList', jsonData.MovieListBonusOff.ShareQueryString);

			bonusComparison(jsonData.Movies);
			theaterCount(jsonData.Movies);
			boxOffice(jsonData.Movies, jsonData.MovieList.Picks[0].BestPerformer);

			inCallback = false;
		},
		error: function () {

			console.log('Error in AJAX call.');

			inCallback = false;
		}
	});
}

// Fill in an already scaffolded mini list with new data.
function movieListMini(movieList, idPrefix, shareQueryString) {
	var pagePiece = movieList.ComparisonHeader == "Bonus ON" ? "On" : "Off";
	var shareAnchor = $('#' + idPrefix + 'SharePicksId');

	if (shareAnchor != null) {
		shareAnchor.attr('href', parseBaseUrl() + shareQueryString);
	}

	setText(idPrefix + 'TotalEarningsId', "Earnings: $" + formatWithCommas(movieList.TotalEarnings));
	setText(idPrefix + 'TotalCostId', movieList.TotalCost + " BUX");

	for (var movieCount = 0, length = movieList.MovieImages.length; movieCount < length; movieCount++) {
		var movie = movieList.Movies[movieCount];
		var image = $('#' + idPrefix + 'MoviePosterId0' + movieCount);
		var tooltip = $('#' + idPrefix + 'TooltipId0' + movieCount);
		var shadowColor = 'grey';

		logit(tooltip);
		logit(movie.Name + ' - $' + formatWithCommas(movie.Earnings));

		if (movie.IsBestPerformer) {
			shadowColor = 'green';
		}

		tooltip.attr('data-original-title', movie.Name + ' - $' + formatWithCommas(movie.Earnings));
		image.attr('src', movieList.MovieImages[movieCount]);
		//image.attr('style', backgroundStyle);
		image.css({ 'box-shadow': '2px 4px 8px 0px ' + shadowColor });
	}

	// Fill in the rest of the slots with blank

	for (var counter = 0; movieCount < 8; movieCount++) {
		var image = $('#' + idPrefix + 'MoviePosterId0' + movieCount);
		var tooltip = $('#' + idPrefix + 'TooltipId0' + movieCount);

		tooltip.attr('data-original-title', 'This screen intentionally left blank - $-2,000,000');
		image.attr('src','/images/MooveePosterBlank.jpg');
		//image.attr('style', 'border-radius: 3px; box-shadow: 2px 4px 8px 0px grey;');
		image.css({ 'box-shadow': '2px 4px 8px 0px grey' });
	}
}

function theaterCount(movies) {

	// Make a SHALLOW clone of the movies array so sort() does not mess with the original array.
	// (cloning the array that contains references)

	var sortedClone = movies.slice(0);

	sortedClone.sort((first, second) => second.TheaterEfficiency - first.TheaterEfficiency);

	for (var movieCount = 0, length = movies.length; movieCount < length; movieCount++) {
		var movie = sortedClone[movieCount];
		var controlIndex = movie.ControlId;
		var boxOffice = $('#boId' + controlIndex);

		// If the last box office edited matches the movie, then HIGHLIGHT it!
		// Otherwise, the top row is light green.
		// Otherwise, even rows are grey and odd rows are white (no color set).

		var backgroundColor = movie.ControlId == global().lastMovieControlIndex ? 'lemonchiffon' : (movieCount % 2 == 0 ? 'lightgrey' : 'white');

		setText('theaterCountName' + movieCount, movie.Name);
		setText('theaterCount' + movieCount, formatWithCommas(movie.TheaterCount));
		setText('theaterCountEarnings' + movieCount, formatWithCommas(movie.Earnings));
		setText('theaterCountEfficiency' + movieCount, formatWithCommas(movie.TheaterEfficiency));

		if (boxOffice != null) {
			var originalValue = boxOffice.attr('data-original-value').replace(/,/g, '');
			var originalCount = boxOffice.attr('data-original-theater-count').replace(/,/g, '');
			var theaterCountDifference = $('#theaterCountDifference' + movieCount);

			if (originalCount == 0) {
				setText('theaterCountLast' + movieCount, '--');
				setText('theaterCountLastEfficiency' + movieCount, '--');
				setText('theaterCountDifference' + movieCount, '--');

				theaterCountDifference.css({ 'color': 'black'});
			}
			else {
				var color = 'black';
				var lastEfficiency = originalValue / originalCount;
				var difference = movie.TheaterCount - originalCount;

				setText('theaterCountLast' + movieCount, formatWithCommas(originalCount));
				setText('theaterCountLastEfficiency' + movieCount, formatWithCommas(lastEfficiency));
				setText('theaterCountDifference' + movieCount, formatWithCommas(difference));

				if (difference > 0) {
					color = '#33cc33';		// green
				}
				else if (difference < 0) {
					color = '#cc3333';		// red
				}

				theaterCountDifference.css({ 'color': color });
			}
		}

		// NOTE: Not very efficient, because you have to retrieve the control using $('#' + id) above and below twice.

		changeStyle('theaterCountName' + movieCount, 'background-color', backgroundColor);
		changeStyle('theaterCount' + movieCount, 'background-color', backgroundColor);
		changeStyle('theaterCountEarnings' + movieCount, 'background-color', backgroundColor);
		changeStyle('theaterCountEfficiency' + movieCount, 'background-color', backgroundColor);
		changeStyle('theaterCountLast' + movieCount, 'background-color', backgroundColor);
		changeStyle('theaterCountLastEfficiency' + movieCount, 'background-color', backgroundColor);
		changeStyle('theaterCountDifference' + movieCount, 'background-color', backgroundColor);
	}
}