using Lumberjack.Constants;
using Lumberjack.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace Lumberjack
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NamedArgumentCaseAnalyzer : LumberjackMessageTemplateArgumentAnalyzer
    {
        public override DiagnosticDescriptor Descriptor => CreateDescriptor(
            "LOG004",
            "Naming rules require arguments to start with upper case",
            null,
            Categories.Logging,
            DiagnosticSeverity.Info,
            true,
            null,
            Array.Empty<string>());

        protected override bool IsInvalidArgument(string argument)
        {
            var charIndex = 0;
            if (argument.Length > 1 && argument[0] == '@')
            {
                charIndex = 1;
            }
            return !int.TryParse(argument, out _) && !char.IsUpper(argument[charIndex]);
        }
    }
}
