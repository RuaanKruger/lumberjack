using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Lumberjack.Core
{
    /// <summary>
    /// This analyzer parses and checks all syntax literals
    /// </summary>
    public abstract class LumberjackLiteralExpressionAnalyzer : LumberjackAnalyzer
    {
        protected abstract bool IsInvalidExpression(LiteralExpressionSyntax literalExpression);

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
                    if (argument.Expression is LiteralExpressionSyntax literal && IsInvalidExpression(literal))
                    {
                        var location = Location.Create(literal.SyntaxTree, TextSpan.FromBounds(literal.Span.Start, literal.Span.End));
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
                    }
                }
            }
        }
    }
}