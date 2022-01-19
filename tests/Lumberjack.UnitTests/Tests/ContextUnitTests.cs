using Lumberjack.UnitTests.Analyzer;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Lumberjack.UnitTests.Tests
{
    public class ContextUnitTest : BaseUnitTest<ContextValidationAnalyzer, EmptyCodeFixProvider>
    {
        [Fact]
        public async Task Analyzer_ShouldExecute_WhenReferenceToLoggingFound()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.Log();
                    }
                }
            ";

            Task TestCode() => Verify(test);
            await Assert.ThrowsAsync<EqualWithMessageException>(TestCode);
        }

        [Fact]
        public async Task Analyzer_ShouldExecute_WhenCategoryLoggerFound()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest
                {
                    public MyTest(ILogger<MyTest> logger) {
                        logger.LogDebug(""Test"");
                    }
                }
            ";

            await Verify(test);
        }

    }
}
