using Xunit;

namespace XUnitTests
{
	public class ParkerMoviePickerTests : MoviePickerTestBase, IClassFixture<ParkerMoviePickerValidationTestsContext>
	{
		public ParkerMoviePickerTests(ParkerMoviePickerValidationTestsContext context)
		{
			Context = context;
		}
	}
}