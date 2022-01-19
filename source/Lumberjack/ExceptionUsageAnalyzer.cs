using Lumberjack.Constants;
using Lumberjack.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Linq;
using Lumberjack.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lumberjack
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ExceptionUsageAnalyzer : LumberjackAnalyzer
    {
        public override DiagnosticDescriptor Descriptor => CreateDescriptor(
            "LOG006",
            "Exceptions must not be part of parameter arguments",
            null,
            Categories.Logging,
            DiagnosticSeverity.Warning,
            true,
            customTags: Array.Empty<string>());

        protected override void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation)
            {
                var symbol = ModelExtensions.GetSymbolInfo(context.SemanticModel, invocation, context.CancellationToken).Symbol;
                if (symbol is IMethodSymbol methodSymbol && methodSymbol.Parameters.Any())
                {
                    var assembly = symbol.ContainingAssembly;
                    if (!assembly.Name.StartsWith("Microsoft.Extensions.Logging", StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }

                    var isParams = methodSymbol.Parameters.Last().IsParams;
                    var parametersIndex = methodSymbol.Parameters.Length;
                    if (!isParams || parametersIndex > invocation.ArgumentList.Arguments.Count)
                    {
                        return;
                    }

                    var exception = context.GetTypeByMetadataName("System.Exception");
                    for (var argumentIndex = parametersIndex - 1; argumentIndex < invocation.ArgumentList.Arguments.Count; argumentIndex++)
                    {
                        var argument = invocation.ArgumentList.Arguments[argumentIndex];
                        var argumentTypeSymbol = ModelExtensions.GetTypeInfo(context.SemanticModel, argument.Expression).Type;
                        if (exception.IsType(argumentTypeSymbol)
                            && !argument.IsKind(SyntaxKind.ParameterList))
                        {
                            Report(context, argument);
                        }
                    }
                }
            }
        }
    }
}
