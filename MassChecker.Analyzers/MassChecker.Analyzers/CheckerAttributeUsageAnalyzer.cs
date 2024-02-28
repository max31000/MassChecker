using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MassChecker.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CheckerAttributeUsageAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.CheckerAttributeUsageError);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
    }

    private static void AnalyzeMethod(SymbolAnalysisContext context)
    {
        var methodSymbol = (IMethodSymbol)context.Symbol;

        // Check if the method has the Checker attribute
        var hasCheckerAttribute =
            methodSymbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == "CheckerAttribute");
        if (!hasCheckerAttribute) return;

        // Check if the containing class inherits from MassCheckerBase<TItem, TFilter>
        var baseType = methodSymbol.ContainingType.BaseType;
        var inheritsFromMassCheckerBase = baseType is
        {
            IsGenericType: true, ConstructedFrom.Name: "MassCheckerBase", TypeArguments.Length: 2
        }; // Ensure there are two generic type arguments

        if (!inheritsFromMassCheckerBase)
        {
            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.CheckerAttributeUsageError,
                methodSymbol.Locations[0], methodSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}