using Lumberjack.Core;
using Lumberjack.UnitTests.Verifiers;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Testing;

namespace Lumberjack.UnitTests
{
    /// <summary>
    /// Convenience methods to reduce amount of code and complexity in child tests
    /// </summary>
    /// <typeparam name="TAnalyzer"></typeparam>
    /// <typeparam name="TCodeFixProvider"></typeparam>
    public class BaseUnitTest<TAnalyzer, TCodeFixProvider> 
        where TAnalyzer : LumberjackAnalyzer, new()
        where TCodeFixProvider : CodeFixProvider, new()
    {

        /// <summary>
        /// Creates an expected diagnostic result with specific line details of the investigation
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private static DiagnosticResult GetExpectedResult(int line, int column)
        {
            var analyzerType = new TAnalyzer();
            return new DiagnosticResult(analyzerType.Descriptor)
                .WithLocation(line, column);
        }

        /// <summary>
        /// Verify that the code expects the diagnostic from type TAnalyzer
        /// </summary>
        /// <param name="test"></param>
        /// <param name="line"></param>
        /// <param name="column"></param>
        protected async Task VerifyAndExpectDiagnostic(string test, int line, int column)
        {
            await CSharpCodeFixVerifier<TAnalyzer, TCodeFixProvider>
                .VerifyAnalyzerAsync(test, 
                    true,
                    GetExpectedResult(line, column));
        }
        
        /// <summary>
        /// Verify that the code expects the diagnostic from type TAnalyzer
        /// </summary>
        /// <param name="test"></param>
        protected async Task VerifyAndExpectDiagnostic(string test)
        {
            var analyzerType = new TAnalyzer();
            await CSharpCodeFixVerifier<TAnalyzer, TCodeFixProvider>
                .VerifyAnalyzerAsync(test, 
                    true,
                    new DiagnosticResult(analyzerType.Descriptor));
        }
        
        /// <summary>
        /// Verify that the code expects the diagnostic from the provided descriptor
        /// </summary>
        /// <param name="test"></param>
        /// <param name="result"></param>
        protected async Task VerifyAndExpectSpecificDiagnostic(string test, DiagnosticResult result)
        {
            await CSharpCodeFixVerifier<TAnalyzer, TCodeFixProvider>
                .VerifyAnalyzerAsync(test, 
                    true, 
                    result);
        }
        
        /// <summary>
        /// Simplest use case : assembly referenced and expected diagnostics are defaulted
        /// </summary>
        /// <param name="test"></param>
        /// <param name="includeLoggingAssemblies"></param>
        /// <param name="expectedDiagnostics"></param>
        protected async Task Verify(string test, bool includeLoggingAssemblies = true, params DiagnosticResult[] expectedDiagnostics)
        {
            await CSharpCodeFixVerifier<TAnalyzer, TCodeFixProvider>
                .VerifyAnalyzerAsync(test, includeLoggingAssemblies, expectedDiagnostics);
        }
    }
}