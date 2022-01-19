using Lumberjack.Constants;
using Lumberjack.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;

namespace Lumberjack
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ExtensionMethodAnalyzer : LumberjackAnalyzer
    {
        public override DiagnosticDescriptor Descriptor => CreateDescriptor(
            "LOG001",
            "Prefer extension method over Log(Level...)",
            "Prefer extension method over Log(Level...)",
            Categories.Logging,
            DiagnosticSeverity.Info,
            true,
            null,
            Array.Empty<string>());

        protected override void AnalyzeOperation(OperationAnalysisContext context, IInvocationOperation invocationOperation, IMethodSymbol method)
        {
            if (method.IsExtensionMethod && method.Name != "Log")
            {
                return;
            }

            Report(context, invocationOperation);
        }
    }
}
