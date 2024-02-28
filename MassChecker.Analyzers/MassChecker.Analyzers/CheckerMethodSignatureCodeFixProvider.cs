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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CheckerMethodSignatureCodeFixProvider)), Shared]
public class CheckerMethodSignatureCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("MC001CheckerMethodSignature");

    public sealed override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics[0];
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the method declaration identified by the diagnostic.
        var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Update method signature to match (TItem, TFilter) -> bool",
                createChangedDocument: c => CorrectMethodSignatureAsync(context.Document, declaration, c),
                equivalenceKey: "UpdateMethodSignature"),
            diagnostic);
    }

    private async Task<Document> CorrectMethodSignatureAsync(Document document, MethodDeclarationSyntax methodDecl, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        // Example transformation: change the method to have two parameters of type object and return type bool
        // This is a simplified example; actual transformation should be based on the specific requirements
        var parameterList = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(new[]
        {
            SyntaxFactory.Parameter(SyntaxFactory.Identifier("item")).WithType(SyntaxFactory.ParseTypeName("TItem")),
            SyntaxFactory.Parameter(SyntaxFactory.Identifier("filter")).WithType(SyntaxFactory.ParseTypeName("TFilter"))
        }));

        var returnType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword));

        editor.ReplaceNode(methodDecl, methodDecl.WithParameterList(parameterList).WithReturnType(returnType));

        return editor.GetChangedDocument();
    }
}