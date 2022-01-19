using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Lumberjack.UnitTests.Tests
{
    public class ExceptionUsageAnalyzerTests : BaseUnitTest<ExceptionUsageAnalyzer, EmptyCodeFixProvider>
    {
        [Fact]
        public async Task ExceptionUsageAnalyzer_ExceptionFirst_NoDiagnostic()
        {
            const string test = @"
                using System;
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogError(new Exception(), ""Something horrible happened"");
                    }
                }
            ";

            await Verify(test);
        }
        
        [Fact]
        public async Task ExceptionUsageAnalyzer_ExceptionAsParameterArrayArgument_ReportDiagnostic()
        {
            const string test = @"
                using System;
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogError(""Something horrible happened"", new Exception());
                    }
                }
            ";

            await VerifyAndExpectDiagnostic(test, 8, 72);
        }
        
        [Fact]
        public async Task ExceptionUsageAnalyzer_ExceptionAsLatterParameterArrayArgument_ReportDiagnostic()
        {
            const string test = @"
                using System;
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogError(""{What} horrible happened "", ""formatter"", ""1"", ""2"", new Exception());
                    }
                }
            ";

            await VerifyAndExpectDiagnostic(test, 8, 93);
        }
    }
}
