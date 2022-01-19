using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Lumberjack.UnitTests.Tests
{
    public class LogMessageAsFragmentAnalyzerTests : BaseUnitTest<LogMessageAsFragmentAnalyzer, EmptyCodeFixProvider>
    {
        [Fact]
        public async Task LogMessageAsFragmentAnalyzer_WhenFragment_NoDiagnostics()
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
        public async Task LogMessageAsFragmentAnalyzer_WhenSentence_ReportDiagnostics()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogDebug(""This is a sentence and violates our rule."");
                    }
                }
            ";

            await VerifyAndExpectDiagnostic(test, 7, 25);
        }

        [Fact]
        public async Task LogMessageAsFragmentAnalyzer_Null_ShouldNotCauseFailures()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogDebug(null);
                    }
                }
            ";

            await Verify(test);
        }
        
        [Fact]
        public async Task LogMessageAsFragmentAnalyzer_InterpolatedExpression_ReportDiagnostics()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogDebug($""This is an {string.Empty} InterpolatedStringTextToken."");
                    }
                }
            ";

            await VerifyAndExpectDiagnostic(test, 7, 25);
        }
    }
}
