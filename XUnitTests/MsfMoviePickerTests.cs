using Xunit;
using Xunit.Abstractions;

namespace XUnitTests
{
	public class MsfMoviePickerTests : MoviePickerTestBase, IClassFixture<MsfMoviePickerValidationTestsContext>
	{
		public MsfMoviePickerTests(ITestOutputHelper outputHelper, MsfMoviePickerValidationTestsContext context)
		{
			OutputHelper = outputHelper;
			Context = context;
		}
	}
}