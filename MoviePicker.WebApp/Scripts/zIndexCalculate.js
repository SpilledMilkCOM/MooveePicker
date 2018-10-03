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

			var totalEarnings = $('#bonusOnMovieListTotalEarningsId');

			if (totalEarnings != null) {
				//totalEarnings.text("Earnings: $" + (jsonData.TotalEarnings).toFixed().replace(/\d(?=(\d{3})+\.)/g, '$&,'));
				totalEarnings.text("Earnings: $" + (jsonData.TotalEarnings).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
			}

			var totalCost = $('#bonusOnMovieListTotalCostId');

			if (totalCost != null) {
				totalCost.text(jsonData.TotalCost + " BUX");
			}

			for (var movieCount = 0, length = jsonData.MovieImages.length; movieCount < length; movieCount++) {
				var image = $('#bonusOnMovieListMoviePosterId0' + movieCount);
				var tooltip = $('#bonusOnMovieListTooltipId0' + movieCount);

				console.log(tooltip);
				console.log(jsonData.Movies[movieCount].Name + ' - $' + (jsonData.Movies[movieCount].Earnings).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));

				tooltip.attr('title', jsonData.Movies[movieCount].Name + ' - $' + (jsonData.Movies[movieCount].Earnings).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
				tooltip.attr('data-original-title', jsonData.Movies[movieCount].Name + ' - $' + (jsonData.Movies[movieCount].Earnings).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
				image.attr('src', jsonData.MovieImages[movieCount]);
			}

			inCallback = false;
		},
		error: function () {

			console.log('Error in AJAX call.');

			inCallback = false;
		}
	});
}