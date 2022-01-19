using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Lumberjack.UnitTests.Tests
{
    public class NamedArgumentCaseAnalyzerTests : BaseUnitTest<NamedArgumentCaseAnalyzer, EmptyCodeFixProvider>
    {
        [Fact]
        public async Task NamedArgumentCaseAnalyzer_WhenSingleCorrectArgument_NoDiagnostics()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogDebug(""{This} is a good example"", string.Empty);
                    }
                }
            ";

            await Verify(test);
        }
        
        [Fact]
        public async Task NamedArgumentCaseAnalyzer_SerilogWhenSingleCorrectArgument_NoDiagnostics()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogDebug(""{@This} is a good example"", string.Empty);
                    }
                }
            ";

            await Verify(test);
        }

        [Fact]
        public async Task NamedArgumentCaseAnalyzer_WhenArgumentIsLower_ReportDiagnostics()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogDebug(""This is {incorrect}"", ""bad"");
                    }
                }
            ";

            await VerifyAndExpectDiagnostic(test, 7, 41);
        }
        
        [Fact]
        public async Task NamedArgumentCaseAnalyzer_SerilogWhenArgumentIsLower_ReportDiagnostics()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogDebug(""This is {@incorrect}"", ""bad"");
                    }
                }
            ";

            await VerifyAndExpectDiagnostic(test, 7, 41);
        }
    }
}
