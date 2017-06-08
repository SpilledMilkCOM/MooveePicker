# Moovee Picker
### A simulation that picks movies.

Given a set of movies with *Earnings* and *Cost* (as well as other rules),
chooses the best lineup of movies to run in your theaters.

This is effectively an "unbounded knapsack" problem.  Added some hashing
(which does not care about the sub-problem order) so not all 2,562,890,625 (15^8) possibilities
are attempted.  That is worst case if all the items have the same weight and value, but some are much
**LARGER** than others and use up more space meaning that there will be **MUCH** less room for
other movies.  You may choose multiples of the same movie provided that it maximizes your
return for the list of movies.

If you remove the hashing, a 3 second algorithm, becomes a 54 minute algorithm (depending on your processor speed).

I'll probably be banned from FML, but oh well.

A reference for the general knapsack (0/1).  (you may **NOT** reuse items, you may only choose
a single instance of an item)
http://www.programminglogic.com/knapsack-problem-dynamic-programming-algorithm/

Reference for many of the knapsack algorithms.
https://en.wikipedia.org/wiki/Knapsack_problem