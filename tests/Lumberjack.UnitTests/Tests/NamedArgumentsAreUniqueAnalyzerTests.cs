using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Lumberjack.UnitTests.Tests
{
    public class NamedArgumentsAreUniqueAnalyzerTests : BaseUnitTest<NamedArgumentsAreUniqueAnalyzer, EmptyCodeFixProvider>
    {
        [Fact]
        public async Task NamedArgumentsAreUniqueAnalyzer_WhenUnique_NoDiagnostics()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogDebug(""{This} is a {Great} example"", string.Empty, string.Empty);
                    }
                }
            ";

            await Verify(test);
        }

        [Fact]
        public async Task NamedArgumentsAreUniqueAnalyzer_WhenDuplicated_ReportDiagnostics()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogDebug(""This is {incorrect} and very {incorrect}"", ""bad"", ""worse"");
                    }
                }
            ";

            await VerifyAndExpectDiagnostic(test, 7, 41);
        }
        
        [Fact]
        public async Task NamedArgumentsAreUniqueAnalyzer_RealWorldExample_ReportDiagnostics()
        {
            const string test = @"
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    public MyTest(ILogger logger) { 
                        logger.LogDebug(""Connection {Connection} closed"", string.Empty);
                    }

                    public MyTest(ILogger logger, string connectionString) { 
                        logger.LogDebug(""Received status update for unknown request. Message: {StatusMessage}. Status: {Diagnostics}"", connectionString, string.Empty);
                    }
                }
            ";

            await Verify(test);
        }
        
        [Fact]
        public async Task NamedArgumentsAreUniqueAnalyzer_DefinedLoggers_NoError()
        {
            const string test = @"
                using System;
                using Microsoft.Extensions.Logging;

                public class MyTest 
                {
                    private static readonly Action<ILogger, object, Exception> LogEnqueueMessage
                        = LoggerMessage.Define<object>(
                            LogLevel.Trace,
                            new EventId(1, string.Empty),
                            ""Enqueue message {Message}"");

                    private static readonly Action<ILogger, object, Exception> LogDequeueMessage
                        = LoggerMessage.Define<object>(
                            LogLevel.Trace,
                            new EventId(2, string.Empty),
                            ""Dequeue message {Message}"");

                }
            ";

            await Verify(test);
        }
    }
}
