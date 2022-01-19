using System;
using Lumberjack.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Lumberjack.Core
{
    /// <summary>
    /// This analyzer parses and checks all arguments within a string that falls within moustaches ex. {x}
    /// </summary>
    public abstract class LumberjackMessageTemplateArgumentAnalyzer : LumberjackAnalyzer
    {
        protected abstract bool IsInvalidArgument(string argument);

        protected override void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is InvocationExpressionSyntax invocation))
            {
                throw new InvalidOperationException("Syntax node is not of type InvocationExpressionSyntax");
            }

            var symbol = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol;
            if (symbol != null)
            {
                var assembly = symbol.ContainingAssembly;
                if (!assembly.Name.StartsWith("Microsoft.Extensions.Logging", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                foreach (var argument in invocation.ArgumentList.Arguments)
                {
                    if (argument.Expression is LiteralExpressionSyntax literal)
                    {
                        var stringArguments = literal.GetArguments();
                        foreach (var stringArgument in stringArguments)
                        {
                            if (string.IsNullOrEmpty(stringArgument))
                            {
                                continue;
                            }

                            if (IsInvalidArgument(stringArgument))
                            {
                                var location = Location.Create(literal.SyntaxTree, TextSpan.FromBounds(literal.Span.Start, literal.Span.End));
                                context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                            }
                        }
                    }
                }
            }
        }
    }
}