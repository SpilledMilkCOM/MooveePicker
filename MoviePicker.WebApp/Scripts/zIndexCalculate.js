var inCallback = false;		// Prevent multiple retrievals of the same data recursion

function calculate(endPoint) {

	if (inCallback == true) {
		// Bust out of the possible multiple calls to this while the callback is attempting to return data.
		return;
	}

	inCallback = true;

	console.log("calculate('" + endPoint + "')");

	$.ajax({
		type: 'GET',
		url: endPoint,
		dataType: 'json',
		success: function (jsonData) {

			if (jsonData == null) {
				inCallback = false;
				console.log('JSON data is null.');
				return;
			}

			// JSON data is a MovieList

			console.log(jsonData);
			console.log(jsonData.TotalEarnings);
			console.log(jsonData.TotalCost);

			movieListMini(jsonData.MovieList.Picks[0], 'bonusOnMovieList');
			movieListMini(jsonData.MovieListBonusOff.Picks[0], 'bonusOffMovieList');

			inCallback = false;
		},
		error: function () {

			console.log('Error in AJAX call.');

			inCallback = false;
		}
	});
}

function formatWithCommas(value) {
	return (value).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

function movieListMini(movieList, idPrefix) {
	setText(idPrefix + 'TotalEarningsId', "Earnings: $" + formatWithCommas(movieList.TotalEarnings));
	setText(idPrefix + 'TotalCostId', movieList.TotalCost + " BUX");

	for (var movieCount = 0, length = movieList.MovieImages.length; movieCount < length; movieCount++) {
		var movie = movieList.Movies[movieCount];
		var image = $('#' + idPrefix + 'MoviePosterId0' + movieCount);
		var tooltip = $('#' + idPrefix +'TooltipId0' + movieCount);
		var backgroundStyle = "border-radius: 3px; box-shadow: 2px 4px 8px 0px grey;";

		console.log(tooltip);
		console.log(movie.Name + ' - $' + formatWithCommas(movie.Earnings));

		if (movie.IsBestPerformer) {
			backgroundStyle = "border-radius: 3px; box-shadow: 2px 4px 8px 0px green;";
		}

		//tooltip.attr('title', movie.Name + ' - $' + formatWithCommas(movie.Earnings));
		tooltip.attr('data-original-title', movie.Name + ' - $' + formatWithCommas(movie.Earnings));
		image.attr('src', movieList.MovieImages[movieCount]);
		image.attr('style', backgroundStyle);
	}
}

function setText(id, value) {
	var ctrl = $('#' + id);

	if (ctrl != null) {
		ctrl.text(value);
	}
}