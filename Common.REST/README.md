# SM.Common.REST
### A C# library for REST API calls

Rolling my own REST Client around the .Net `WebClient`

I'm trying to stay away from inheritence, so the `AddHeaders()` method is exposed so the class
that uses this can wire up the headers externally.