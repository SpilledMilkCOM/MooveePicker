# ![MooVee Picker Logo](https://mooveepicker.com/Images/Moovee%20Picker%20Cow64x64.png) MooVee Picker
### A simulation that picks movies.

Given a set of movies with *Earnings* and *Cost* (as well as other rules),
chooses the best lineup of movies to run in your theaters.  [MooVee Picker](https://mooveepicker.com) allows you
to use industry expert data as a basis for choosing your own line-up in the [Fantasy Movie League](https://fantasymovieleague.com/) game.

##### :computer: Basic Algorithm

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

##### Reference

A reference for the general knapsack (0/1).  (you may **NOT** reuse items, you may only choose
a single instance of an item)
http://www.programminglogic.com/knapsack-problem-dynamic-programming-algorithm/

Reference for many of the knapsack algorithms.
https://en.wikipedia.org/wiki/Knapsack_problem

Microsoft Solver Foundation:
https://msdn.microsoft.com/en-us/library/ff524497(v=vs.93).aspx

Actual published website:
http://mooveepicker.com/

Tricks for styling sliders (input type range controls)
https://css-tricks.com/styling-cross-browser-compatible-range-inputs-css/

Using The Movie Database for movie posters where needed.
http://www.themoviedb.org/

The current defaults that I use are on the [About](http://mooveepicker.com/home/about) page.


##### Things I learned in this project
* More Javascript and JQuery shortcuts
  * `.sort()` on arrays (ascending/descending)
  * `.slice(0)` to make a shallow clone of array objects.
* I didn't know that hsl() was a thing in CSS (pretty cool)
* More about Bootstrap
  * Columns within columns.
  * Grids with headers which switch to labels in mobile view.
* Twitter
  * Large summary cards are clipped, supposed to be 2:1 aspect but 600x314 works better
  * [Emoji's](https://www.piliapp.com/twitter-symbols/) :smiley:
* Phone browsers are weird.
* Browsers support PNG file format as a "[favicon](https://en.wikipedia.org/wiki/Favicon)" (don't need the old ICO file anymore - YAY!)
* How to add Application Insights to monitor the web app.
* How to add Adsterra.com ads (removed them due to ads that take over your browser - NOT GOOD)
* More about [Open Graph](http://ogp.me/) Facebook's standard for web page metadata.
* Kusto Query Language (KQL) queries for Azure Application Insights logs.
  * A *GREAT* training reference: [Kusto Query Language (KQL) from Scratch](https://app.pluralsight.com/course-player?clipId=b9fc66c1-8f47-4d14-af5f-a6758b86ff2f)
* Send email using [SendGrid ](https://sendgrid.com/) since there's no SMTP Server for an Azure Application
    (unless you have a virtual machine for it).

##### Azure Notes
* Application Settings
  * Change from default 32 bit to 64 bit
  * Turned OFF ARR Affinity to improve performance
  * Added connection string here
* Added some alerts
  * A classic alert for memory
  * A new alert that encomasses just about everything else.
* Added an auto scale out in the App Service Plan  >80% memory +1 instance.
  * Added notification
* There is no default to scale the instances back down.  Had to add one when memory is <10% decrement an instance.
* Added Key Vault and Certificate to get the service secure using HTTPS.