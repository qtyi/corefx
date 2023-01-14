// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Operations;
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
        var document = context.Document;
        var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null) return;

        var diagnostic = context.Diagnostics.First();
        if (!LangFieldAttributeDiagnosticAnalyzer.TryGetProperties(diagnostic.Properties,
            out var expectedTargets,
            out var actualTargets,
            out var elseInherited)) return;
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var model = await document.GetSemanticModelAsync(context.CancellationToken);
        if (model is null) return;

        var codeAction = CodeAction.Create("删除不符合的标志项", FixAttributeArgument);
        if (!elseInherited)
        {
            codeAction = CodeAction.Create($"修改错误的{nameof(AttributeUsageAttribute)}", ImmutableArray.Create(
                codeAction,
                CodeAction.Create($"删除{nameof(AttributeUsageAttribute)}以继承设置", RemoveAttribute)
            ), false);
        }
        context.RegisterCodeFix(codeAction, diagnostic);

        static T? AncestorAndSelf<T>(IOperation? op) where T : class, IOperation
        {
            while (op is not null)
            {
                if (op is T tOp) return tOp;
                op = op.Parent;
            }
            return null;
        }

        async Task<Document> FixAttributeArgument(CancellationToken cancellationToken)
        {
            var aOp = AncestorAndSelf<IAttributeOperation>(model.GetOperation(root.FindNode(diagnosticSpan), cancellationToken));
            var ocOp = aOp?.Operation as IObjectCreationOperation;
            var arg1 = ocOp?.Arguments.FirstOrDefault();
            if (arg1 is null) return document;

            SyntaxNode oldNode, newNode;
            oldNode = arg1.Syntax;
            if (root.Language == LanguageNames.CSharp)
            {
                newNode = CreateCSharpAttributeArgument(expectedTargets);
            }
            else if (root.Language == LanguageNames.VisualBasic)
            {
                newNode = CreateVisualBasicAttributeArgument(expectedTargets);
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
            var aOp = AncestorAndSelf<IAttributeOperation>(model.GetOperation(root.FindNode(diagnosticSpan), cancellationToken));
            if (aOp is null) return document;

            var node = aOp.Syntax;

            if (cancellationToken.IsCancellationRequested) return document;

            var newRoot = root.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);
            if (newRoot is null) return document;

            if (cancellationToken.IsCancellationRequested) return document;

            return document.WithSyntaxRoot(newRoot);
        }
    }

    private CS.Syntax.AttributeArgumentSyntax CreateCSharpAttributeArgument(AttributeTargets expectedTargets)
    {
        var type = typeof(AttributeTargets);
        var operands = expectedTargets.GetFlags().DefaultIfEmpty(AttributeTargets.All).Select(flag =>
            (CS.Syntax.ExpressionSyntax)CS.SyntaxFactory.MemberAccessExpression(
                CS.SyntaxKind.SimpleMemberAccessExpression,
                CS.SyntaxFactory.ParseTypeName(type.FullName),
                CS.SyntaxFactory.IdentifierName(Enum.GetName(type, flag))
            )
        ).ToArray();

        if (operands.Length == 1)
            return CS.SyntaxFactory.AttributeArgument(operands[0]);
        else
            return CS.SyntaxFactory.AttributeArgument(
                operands.Skip(1).Aggregate(
                    operands[0],
                    (left, right) => CS.SyntaxFactory.BinaryExpression(CS.SyntaxKind.BitwiseOrExpression, left, right)
                )
            );
    }

    private VB.Syntax.ArgumentSyntax CreateVisualBasicAttributeArgument(AttributeTargets expectedTargets)
    {
        var type = typeof(AttributeTargets);
        var operands = expectedTargets.GetFlags().DefaultIfEmpty(AttributeTargets.All).Select(flag =>
            (VB.Syntax.ExpressionSyntax)VB.SyntaxFactory.SimpleMemberAccessExpression(
                VB.SyntaxFactory.ParseTypeName(type.FullName),
                VB.SyntaxFactory.Token(VB.SyntaxKind.DotToken),
                VB.SyntaxFactory.IdentifierName(Enum.GetName(type, flag))
            )
        ).ToArray();

        if (operands.Length == 1)
            return VB.SyntaxFactory.SimpleArgument(operands[0]);
        else
            return VB.SyntaxFactory.SimpleArgument(
                operands.Skip(1).Aggregate(
                    operands[0],
                    (left, right) => VB.SyntaxFactory.BinaryExpression(
                        VB.SyntaxKind.OrExpression,
                        left,
                        VB.SyntaxFactory.Token(VB.SyntaxKind.OrKeyword),
                        right
                    )
                )
            );
    }
}
