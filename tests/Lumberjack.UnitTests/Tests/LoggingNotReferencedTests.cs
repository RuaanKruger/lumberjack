using Lumberjack.UnitTests.Analyzer;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Lumberjack.UnitTests.Tests
{
    public class LoggingNotReferencedTests : BaseUnitTest<ContextValidationAnalyzer, EmptyCodeFixProvider>
    {
        [Fact]
        public async Task Analyzer_ShouldNotExecute_WhenEmpty()
        {
            var test = string.Empty;
            await Verify(test, false);
        }

        [Fact]
        public async Task Analyzer_ShouldNotExecute_WhenLoggingNotFound()
        {
            const string test = @"
                public class MyTest 
                {
                    
                }
            ";
            await Verify(test, false);
        }
    }
}
