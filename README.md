# Moovee Picker
### A simulation that picks movies.

Given a set of movies with Earnings and Cost (as well as other rules),
chooses the best lineup of movies to run in your theaters.

This is effectively a knapsack problem, but will try to add some some
smarter trimming so not all 2,562,890,625 (15^8) possibilities are attempted.