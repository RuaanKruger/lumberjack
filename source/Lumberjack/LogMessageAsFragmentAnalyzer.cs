using Lumberjack.Constants;
using Lumberjack.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Linq;

namespace Lumberjack
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LogMessageAsFragmentAnalyzer : LumberjackAnalyzer
    {
        private const int StartIndex = 2;

        public override DiagnosticDescriptor Descriptor => CreateDescriptor(
            "LOG002",
            "Avoid trailing period in messages, because log messages are fragments, not sentences.",
            null,
            Categories.Logging,
            DiagnosticSeverity.Info,
            true,
            null,
            Array.Empty<string>());

        protected override void AnalyzeOperation(OperationAnalysisContext context, IInvocationOperation invocationOperation, IMethodSymbol method)
        {
            if (invocationOperation?.Arguments.Length == 0)
            {
                return;
            }

            var messageParameter = invocationOperation?.Arguments.FirstOrDefault(x => x.Parameter?.Name == "message");
            if (messageParameter != null)
            {
                var messageText = messageParameter.Value.ConstantValue.ToString();

                // Interpolated text
                if (messageParameter.Value.Kind == OperationKind.InterpolatedString)
                {
                    var interpolatedString = messageParameter.Syntax.ToString();
                    messageText = interpolatedString.Substring(StartIndex, interpolatedString.Length - (1 + StartIndex));
                }

                if (messageText.EndsWith("."))
                {
                    Report(context, invocationOperation);
                }
            }
        }
    }
}
