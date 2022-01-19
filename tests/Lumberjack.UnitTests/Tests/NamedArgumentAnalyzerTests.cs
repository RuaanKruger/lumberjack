using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Lumberjack.UnitTests.Tests
{
    public class NamedArgumentAnalyzerTests : BaseUnitTest<NamedArgumentAnalyzer, EmptyCodeFixProvider>
    {
        [Fact]
        public async Task NamedArgumentAnalyzer_WhenSingleCorrectArgument_NoDiagnostics()
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
        public async Task NamedArgumentAnalyzer_WhenMultipleCorrectArgument_NoDiagnostics()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogDebug(""{This} is a good {Example}"", string.Empty, string.Empty);
                    }
                }
            ";

            await Verify(test);
        }

        [Fact]
        public async Task NamedArgumentAnalyzer_SingleNonNamedArgument_ReportDiagnostics()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        var s = string.Format("" {0} "", 1);
                        logger.LogDebug(""This is a {0} example"", ""bad"");
                    }
                }
            ";

            await VerifyAndExpectDiagnostic(test, 8, 41);
        }

        [Fact]
        public async Task NamedArgumentAnalyzer_Null_ShouldNotCauseFailures()
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
        public async Task NamedArgumentAnalyzer_Exception_ShouldNotCauseFailures()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;
                using System;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        throw new Exception(string.Format("" Now = {0} "", DateTime.Now));
                    }
                }
            ";

            await Verify(test);
        }
    }
}
