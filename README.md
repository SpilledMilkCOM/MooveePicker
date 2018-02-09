# Moovee Picker
### A simulation that picks movies.

Given a set of movies with *Earnings* and *Cost* (as well as other rules),
chooses the best lineup of movies to run in your theaters.

##### Basic Algorithm

This is effectively an "unbounded knapsack" problem.  Added some hashing
(which does not care about the sub-problem order) so not all 4,294,967,296 (16^8) possibilities
are attempted.  That is worst case if all the items have the same weight and value, but some are much
**LARGER** than others and use up more space meaning that there will be **MUCH** less room for
other movies.  You may choose multiples of the same movie provided that it maximizes your
return for the list of movies.

If you remove the hashing, a 3 second algorithm, becomes a 54 minute algorithm (depending on your processor speed).

##### A More Advanced Algorithm

Using the above algorithm to chose the **best** from a list, this second algorithm manufactures **MANY** lists
based on adjusting the earnings up or down a specified amount.  By doing so you can be **more** confident that
your baseline is going to be pretty robust.  If there are too many discrepancies from your baseline, then
you might want to rethink your values or just be fine with that baseline.

##### Speculation

Another class that will find the value where your "tent pole" movie becomes irrelevant.

##### Aside

I'll probably be banned from FML, but oh well.  It's still not an exact science...  until it **IS**.

##### Reference

A reference for the general knapsack (0/1).  (you may **NOT** reuse items, you may only choose
a single instance of an item)
http://www.programminglogic.com/knapsack-problem-dynamic-programming-algorithm/

Reference for many of the knapsack algorithms.
https://en.wikipedia.org/wiki/Knapsack_problem

Microsoft Solver Foundation:
https://msdn.microsoft.com/en-us/library/ff524497(v=vs.93).aspx

Actual published website:
http://mooveepicker.azurewebsites.net/

The current defaults that I use:
http://mooveepicker.azurewebsites.net/home/picks?wl=0,3,3,3,1,1,6

##### Things I learned in this project
* More Javascript and shortcuts
* I didn't know that hsl() was a thing in CSS (pretty cool)
* More about Bootstrap and columns within columns.
* Phone browsers are weird.