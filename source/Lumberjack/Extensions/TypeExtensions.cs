using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Lumberjack.Extensions
{
    /// <summary>
    /// Extensions : Type
    /// </summary>
    public static class TypeExtensions
    {
        public static INamedTypeSymbol GetTypeByMetadataName(this SyntaxNodeAnalysisContext context, string fullyQualifiedMetadataName)
        {
            return context.SemanticModel.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
        }

        public static TypeInfo GetTypeInfo(this SyntaxNodeAnalysisContext context, SyntaxNode node, CancellationToken cancellationToken = default)
        {
            return context.SemanticModel.GetTypeInfo(node, cancellationToken);
        }

        public static bool IsType(this ITypeSymbol typeSymbol, ITypeSymbol type)
        {
            if (typeSymbol == null)
            {
                return false;
            }

            for (var symbol = type; symbol != null; symbol = symbol.BaseType)
            {
                if (SymbolEqualityComparer.Default.Equals(symbol, typeSymbol))
                {
                    return true;
                }
            }
            return false;
        }
    }
}