using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MassChecker.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ConditionMethodSignatureAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.CheckerMethodSignatureIncorrect);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        var attributeLists = methodDeclaration.AttributeLists;
        foreach (var attributeList in attributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var attributeSymbol = context.SemanticModel.GetSymbolInfo(attribute).Symbol?.ContainingSymbol;
                if (attributeSymbol?.ToDisplayString() == "MassChecker.CheckerAttribute")
                {
                    var parameters = methodDeclaration.ParameterList.Parameters;
                    var returnType = methodDeclaration.ReturnType;

                    // Check if the method signature matches (TItem, TFilter) -> bool
                    if (parameters.Count != 2 || !returnType.IsKind(SyntaxKind.PredefinedType) ||
                        ((PredefinedTypeSyntax)returnType).Keyword.ValueText != "bool")
                    {
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.CheckerMethodSignatureIncorrect, methodDeclaration.Identifier.GetLocation(), methodDeclaration.Identifier.ValueText);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}