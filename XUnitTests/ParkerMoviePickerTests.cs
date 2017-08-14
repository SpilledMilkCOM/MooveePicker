using Xunit;
using Xunit.Abstractions;

namespace XUnitTests
{
    public class ParkerMoviePickerTests : MoviePickerTestBase, IClassFixture<ParkerMoviePickerValidationTestsContext>
    {
        public ParkerMoviePickerTests(ITestOutputHelper outputHelper, ParkerMoviePickerValidationTestsContext context)
            : base(outputHelper, context)
        {

        }
    }
}