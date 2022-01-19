using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lumberjack.Extensions
{
    /// <summary>
    /// Extensions : Literals
    /// </summary>
    public static class LiteralExtensions
    {
        private static readonly Regex s_ArgumentRegex = new Regex("{(.*?)}", RegexOptions.Compiled);

        public static IEnumerable<string> GetArguments(this LiteralExpressionSyntax literalExpressionSyntax)
        {
            if (literalExpressionSyntax == null)
            {
                throw new ArgumentNullException(nameof(literalExpressionSyntax));
            }

            var matches = s_ArgumentRegex.Matches(literalExpressionSyntax.Token.Text);
            var stringArguments = new List<string>();

            foreach (Match match in matches)
            {
                if (!string.IsNullOrEmpty(match.Groups[1].Value))
                {
                    stringArguments.Add(match.Groups[1].Value);
                }
            }
            return stringArguments.ToArray();
        }
    }
}