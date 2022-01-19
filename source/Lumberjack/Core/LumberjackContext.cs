using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace Lumberjack.Core
{
    public class LumberjackContext
    {
        public LumberjackContext(Compilation compilation)
        {
            if (compilation == null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            IsLoggingReferenced = compilation.ReferencedAssemblyNames.Any(a => a.Name.Equals("microsoft.extensions.logging",
                StringComparison.OrdinalIgnoreCase));

            IsLoggingTypeFound = compilation.GetTypeByMetadataName("Microsoft.Extensions.Logging.ILogger") != null;
        }

        public bool IsLoggingReferenced { get; }

        public bool IsLoggingTypeFound { get; }
    }
}
