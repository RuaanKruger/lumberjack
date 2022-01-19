using Lumberjack.Constants;
using Lumberjack.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace Lumberjack.UnitTests.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContextValidationAnalyzer : LumberjackAnalyzer
    {
        public override DiagnosticDescriptor Descriptor => CreateDescriptor(
            "TEST-001",
            "Test that our detection of our analyzers work as expected",
            null,
            Categories.Logging,
            DiagnosticSeverity.Info,
            false,
            null,
            Array.Empty<string>());
    }
}
