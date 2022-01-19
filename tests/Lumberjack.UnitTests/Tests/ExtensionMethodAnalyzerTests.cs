using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Lumberjack.UnitTests.Tests
{
    public class ExtensionMethodAnalyzerTests : BaseUnitTest<ExtensionMethodAnalyzer, EmptyCodeFixProvider>
    {
        [Fact]
        public async Task ExtensionMethodAnalyzer_WhenExtensionMethodUsed_NoDiagnostics()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogDebug(string.Empty);
                    }
                }
            ";

            await Verify(test);
        }

        [Fact]
        public async Task ExtensionMethodAnalyzer_WhenMethodUsed_FailDiagnostics()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.Log(LogLevel.Information, string.Empty);
                    }
                }
            ";

            await VerifyAndExpectDiagnostic(test, 7, 25);
        }
    }
}
