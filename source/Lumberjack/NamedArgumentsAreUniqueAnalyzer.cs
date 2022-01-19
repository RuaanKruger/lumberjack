using Lumberjack.Constants;
using Lumberjack.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Diagnostics;
using System.Linq;
using Lumberjack.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lumberjack
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NamedArgumentsAreUniqueAnalyzer : LumberjackLiteralExpressionAnalyzer
    {
        public override DiagnosticDescriptor Descriptor => CreateDescriptor(
            "LOG005",
            "Named arguments must be unique",
            null,
            Categories.Logging,
            DiagnosticSeverity.Warning,
            true,
            null,
            Array.Empty<string>());

        protected override bool IsInvalidExpression(LiteralExpressionSyntax literalExpression)
        {
            if (!(literalExpression.GetArguments() is string[] stringArguments) || stringArguments.Length <= 1)
            {
                return false;
            }

            Debug.WriteLine(string.Join(",", stringArguments));
            var duplicates = stringArguments
                .GroupBy(_ => _)
                .Where(_ => _.Count() > 1);

            return duplicates.Any();
        }
    }
}
