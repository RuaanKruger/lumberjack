using Lumberjack.Constants;
using Lumberjack.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace Lumberjack
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NamedArgumentAnalyzer : LumberjackMessageTemplateArgumentAnalyzer
    {
        public override DiagnosticDescriptor Descriptor => CreateDescriptor(
            "LOG003",
            "Prefer named arguments over numeric positions",
            null,
            Categories.Logging,
            DiagnosticSeverity.Warning,
            true,
            null,
            Array.Empty<string>());

        protected override bool IsInvalidArgument(string argument)
        {
            return int.TryParse(argument, out _);
        }
    }
}
