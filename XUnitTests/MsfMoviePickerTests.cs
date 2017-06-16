using Xunit;

namespace XUnitTests
{
	public class MsfMoviePickerTests : MoviePickerTestBase, IClassFixture<MsfMoviePickerValidationTestsContext>
	{
		public MsfMoviePickerTests(MsfMoviePickerValidationTestsContext context)
		{
			Context = context;
		}
	}

	public class ParkerMoviePickerTests : MoviePickerTestBase, IClassFixture<ParkerMoviePickerValidationTestsContext>
	{
		public ParkerMoviePickerTests(ParkerMoviePickerValidationTestsContext context)
		{
			Context = context;
		}
	}
}