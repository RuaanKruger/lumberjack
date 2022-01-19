﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Lumberjack.UnitTests.Verifiers
{
    public static partial class CSharpCodeFixVerifier<TAnalyzer, TCodeFix>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic()"/>
        public static DiagnosticResult Diagnostic()
            => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, XUnitVerifier>.Diagnostic();

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic(string)"/>
        public static DiagnosticResult Diagnostic(string diagnosticId)
            => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, XUnitVerifier>.Diagnostic(diagnosticId);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic(DiagnosticDescriptor)"/>
        public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => CSharpCodeFixVerifier<TAnalyzer, TCodeFix, XUnitVerifier>.Diagnostic(descriptor);

        /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])"/>
        public static async Task VerifyAnalyzerAsync(string source, bool includeAssemblyReferences = true, params DiagnosticResult[] expected)
        {
            var test = new Test
            {
                TestCode = source
            };

            if (includeAssemblyReferences)
            {
                AddAssemblyReferences(test);
            }

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None);
        }

        public static async Task VerifyCodeFixAsync(string source, string fixedSource, bool includeAssemblyReferences)
            => await VerifyCodeFixAsync(source, DiagnosticResult.EmptyDiagnosticResults, fixedSource, includeAssemblyReferences);

        public static async Task VerifyCodeFixAsync(string source, DiagnosticResult expected, string fixedSource, bool includeAssemblyReferences)
            => await VerifyCodeFixAsync(source, new[] { expected }, fixedSource, includeAssemblyReferences);

        private static async Task VerifyCodeFixAsync(string source, IEnumerable<DiagnosticResult> expected, string fixedSource, bool includeAssemblyReferences)
        {
            var test = new Test
            {
                TestCode = source,
                FixedCode = fixedSource

            };

            if (includeAssemblyReferences)
            {
                AddAssemblyReferences(test);
            }

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None);
        }

        private static void AddAssemblyReferences(Test test)
        {
            if (test == null)
            {
                return;
            }

            test.ReferenceAssemblies = ReferenceAssemblies.Default.AddPackages(ImmutableArray.Create(
                   new PackageIdentity("Microsoft.Extensions.Logging", "5.0.0")));
        }
    }
}
