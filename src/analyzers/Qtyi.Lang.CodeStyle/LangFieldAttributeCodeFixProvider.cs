// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using CS = Microsoft.CodeAnalysis.CSharp;
using VB = Microsoft.CodeAnalysis.VisualBasic;

namespace Qtyi.CodeAnalysis;

[System.Composition.Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, LanguageNames.VisualBasic, Name = nameof(LangFieldAttributeCodeFixProvider))]
public sealed class LangFieldAttributeCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
        LangFieldAttributeDiagnosticAnalyzer.s_UnexpectedAttributeTargets.Id
    );

    public override FixAllProvider? GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
#if false
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
#endif
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return;

        var diagnostic = context.Diagnostics.First();
        if (!LangFieldAttributeDiagnosticAnalyzer.TryGetProperties(diagnostic.Properties,
            out var expectedTargets,
            out var actualTargets,
            out var elseInherited)) return;
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var containingNodes = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf();
        if (containingNodes is null) return;

        var document = context.Document;
        var codeAction = CodeAction.Create("删除不符合的标志项", FixAttributeArgument);
        if (!elseInherited)
        {
            codeAction = CodeAction.Create($"修改错误的{nameof(AttributeUsageAttribute)}", ImmutableArray.Create(
                codeAction,
                CodeAction.Create($"删除{nameof(AttributeUsageAttribute)}以继承设置", RemoveAttribute)
            ), false);
        }
        context.RegisterCodeFix(codeAction, diagnostic);

        async Task<Document> FixAttributeArgument(CancellationToken cancellationToken)
        {
            SyntaxNode oldNode, newNode;
            if (root.Language == LanguageNames.CSharp)
            {
                var argument = containingNodes.OfType<CS.Syntax.AttributeArgumentSyntax>().FirstOrDefault();
                if (argument is null) return document;

                if (cancellationToken.IsCancellationRequested) return document;

                oldNode = argument;
                newNode = await FixedCSharpAttributeArgument(argument, cancellationToken);
            }
            else if (root.Language == LanguageNames.VisualBasic)
            {
                var argument = containingNodes.OfType<VB.Syntax.ArgumentSyntax>().FirstOrDefault();
                if (argument is null) return document;

                if (cancellationToken.IsCancellationRequested) return document;

                oldNode = argument;
                newNode = await FixedVisualBasicAttributeArgument(argument, cancellationToken);
            }
            else
            {
                Debug.Assert(false);
                return document;
            }

            if (cancellationToken.IsCancellationRequested) return document;

            var newRoot = root.ReplaceNode(oldNode, newNode);

            if (cancellationToken.IsCancellationRequested) return document;

            return document.WithSyntaxRoot(newRoot);
        }

        async Task<Document> RemoveAttribute(CancellationToken cancellationToken)
        {
            SyntaxNode node;
            if (root.Language == LanguageNames.CSharp)
            {
                var attribute = containingNodes.OfType<CS.Syntax.AttributeSyntax>().FirstOrDefault();
                if (attribute is null) return document;

                node = attribute;
            }
            else if (root.Language == LanguageNames.VisualBasic)
            {
                var attribute = containingNodes.OfType<VB.Syntax.AttributeSyntax>().FirstOrDefault();
                if (attribute is null) return document;

                node = attribute;
            }
            else
            {
                Debug.Assert(false);
                return document;
            }

            if (cancellationToken.IsCancellationRequested) return document;

            var newRoot = root.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);
            if (newRoot is null) return document;

            if (cancellationToken.IsCancellationRequested) return document;

            return document.WithSyntaxRoot(newRoot);
        }
    }

    private async Task<CS.Syntax.AttributeArgumentSyntax> FixedCSharpAttributeArgument(CS.Syntax.AttributeArgumentSyntax attributeArgument, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task<CS.Syntax.AttributeArgumentSyntax> FixedVisualBasicAttributeArgument(VB.Syntax.ArgumentSyntax attributeArgument, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
