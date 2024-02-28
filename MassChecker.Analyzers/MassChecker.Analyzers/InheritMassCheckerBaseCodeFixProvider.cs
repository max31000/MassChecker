using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace MassChecker.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InheritMassCheckerBaseCodeFixProvider)), Shared]
public class InheritMassCheckerBaseCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds => 
        ImmutableArray.Create("MC002CheckerAttributeUsage");

    public sealed override FixAllProvider GetFixAllProvider() => 
        WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var methodDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (methodDeclaration == null || methodDeclaration.ParameterList.Parameters.Count != 2) return;

        var classDeclaration = methodDeclaration.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (classDeclaration == null) return;

        context.RegisterCodeFix(
            CodeAction.Create(
                "Inherit from MassCheckerBase with appropriate types",
                c => ImplementInheritanceAsync(context.Document, classDeclaration, methodDeclaration, c),
                nameof(InheritMassCheckerBaseCodeFixProvider)),
            diagnostic);
    }

    private async Task<Document> ImplementInheritanceAsync(Document document, ClassDeclarationSyntax classDecl, MethodDeclarationSyntax methodDecl, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
        
        // Extract types from the method signature
        var parameterTypes = methodDecl.ParameterList.Parameters.Select(p => p.Type)
            .Where(x => x != null)
            .Select(x => x!)
            .ToList();

        if (parameterTypes.Count != 2)
            return document; // Ensure there are exactly two parameters

        var genericName = SyntaxFactory.GenericName(
            SyntaxFactory.Identifier("MassCheckerBase"),
            SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(parameterTypes))
        );

        var baseList = classDecl.BaseList ?? SyntaxFactory.BaseList();
        var newBaseList = baseList.AddTypes(SyntaxFactory.SimpleBaseType(genericName));

        editor.ReplaceNode(classDecl, classDecl.WithBaseList(newBaseList));

        return editor.GetChangedDocument();
    }
}