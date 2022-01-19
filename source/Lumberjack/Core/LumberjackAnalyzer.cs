using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lumberjack.Core
{
    /// <summary>
    /// Top level analyzer
    /// </summary>
    public abstract class LumberjackAnalyzer : DiagnosticAnalyzer
    {
        private const string HelpUri = "https://github.com/RuaanKruger/lumberjack/blob/main/docs/cs/{0}.md";

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            LumberjackContext lumberjackContext;
            context.RegisterCompilationStartAction(compilationContext =>
            {
                lumberjackContext = new LumberjackContext(compilationContext.Compilation);
                if (IsLoggingAvailableInClass(lumberjackContext))
                {
                    // Operation
                    compilationContext.RegisterOperationAction(operationContext =>
                    {
                        var invocationOperation = (IInvocationOperation)operationContext.Operation;
                        var methodSymbol = invocationOperation.TargetMethod;

                        if (IsLoggerInvoked(methodSymbol))
                        {
                            AnalyzeOperation(operationContext, invocationOperation, methodSymbol);
                        }
                    }, OperationKind.Invocation);

                    // Syntax
                    compilationContext.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.InvocationExpression);
                }
            });
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => new[] { Descriptor }.ToImmutableArray();

        /// <summary>
        /// Determines if the invocation is on the logging namespace
        /// </summary>
        /// <param name="methodSymbol"></param>
        /// <returns></returns>
        private static bool IsLoggerInvoked(ISymbol methodSymbol)
        {
            return methodSymbol?.ContainingType.ContainingNamespace.Name == "Logging";
        }

        /// <summary>
        /// Operation
        /// </summary>
        /// <param name="context"></param>
        /// <param name="invocationOperation"></param>
        /// <param name="method"></param>
        protected virtual void AnalyzeOperation(OperationAnalysisContext context, IInvocationOperation invocationOperation, IMethodSymbol method)
        {
        }

        /// <summary>
        /// Determines if logging is referenced
        /// </summary>
        /// <param name="lumberjackContext"></param>
        /// <returns>True if the </returns>
        private static bool IsLoggingAvailableInClass(LumberjackContext lumberjackContext) =>
            lumberjackContext.IsLoggingReferenced && lumberjackContext.IsLoggingTypeFound;

        /// <summary>
        /// Syntax
        /// </summary>
        /// <param name="context"></param>
        protected virtual void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
        }

        /// <summary>
        /// Override in order to return the analyzers descriptor
        /// </summary>
        public virtual DiagnosticDescriptor Descriptor => null;

        /// <summary>
        /// Utility method for creating descriptor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="messageFormat"></param>
        /// <param name="category"></param>
        /// <param name="defaultSeverity"></param>
        /// <param name="isEnabledByDefault"></param>
        /// <param name="description"></param>
        /// <param name="customTags"></param>
        /// <returns></returns>
        protected static DiagnosticDescriptor CreateDescriptor(
            string id,
            string title,
            string messageFormat,
            string category,
            DiagnosticSeverity defaultSeverity,
            bool isEnabledByDefault,
            string description = null,
            params string[] customTags)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return new DiagnosticDescriptor(
                id,
                title,
                messageFormat ?? title,
                category,
                defaultSeverity,
                isEnabledByDefault,
                description,
                string.Format(HelpUri, id),
                customTags);
        }

        /// <summary>
        /// Report diagnostic
        /// </summary>
        /// <param name="context"></param>
        /// <param name="invocationOperation"></param>
        protected void Report(OperationAnalysisContext context, IInvocationOperation invocationOperation)
        {
            if (invocationOperation == null)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocationOperation.Syntax.GetLocation()));
        }

        /// <summary>
        /// Report diagnostic
        /// </summary>
        /// <param name="context"></param>
        /// <param name="argument"></param>
        protected void Report(SyntaxNodeAnalysisContext context, ArgumentSyntax argument)
        {
            if (argument == null)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptor, argument.GetLocation(), argument.Expression.ToFullString()));
        }
    }
}
