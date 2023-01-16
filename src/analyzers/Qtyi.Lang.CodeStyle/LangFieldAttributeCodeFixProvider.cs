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

        var codeAction = CodeAction.Create($"删除不符合的{nameof(AttributeTargets)}标志项", FixAttributeArgument);
        if (elseInherited)
        {
            codeAction = CodeAction.Create($"修改错误的{nameof(AttributeUsageAttribute)}", ImmutableArray.Create(
                codeAction,
                CodeAction.Create($"删除{nameof(AttributeUsageAttribute)}以继承设置", RemoveAttribute)
            ), isInlinable: false);
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
                newNode = CreateCSharpAttributeArgument(expectedTargets, root, model.Compilation, cancellationToken);
            }
            else if (root.Language == LanguageNames.VisualBasic)
            {
                newNode = CreateVisualBasicAttributeArgument(expectedTargets, root);
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
            if (root.Language == LanguageNames.CSharp)
            {
                var list = (CS.Syntax.AttributeListSyntax)node.Parent!;
                if (list.Attributes.Count == 1)
                    node = list;
            }
            else if (root.Language == LanguageNames.VisualBasic)
            {
                var list = (VB.Syntax.AttributeListSyntax)node.Parent!;
                if (list.Attributes.Count == 1)
                    node = list;
            }
            else
            {
                Debug.Assert(false);
                return document;
            }

            if (cancellationToken.IsCancellationRequested) return document;

            var newRoot = root.RemoveNode(node, SyntaxRemoveOptions.KeepEndOfLine);
            if (newRoot is null) return document;

            if (cancellationToken.IsCancellationRequested) return document;

            return document.WithSyntaxRoot(newRoot);
        }
    }

    private CS.Syntax.AttributeArgumentSyntax CreateCSharpAttributeArgument(AttributeTargets expectedTargets, SyntaxNode root, Compilation compilation, CancellationToken cancellationToken)
    {
        var type = typeof(AttributeTargets);
        var operands = expectedTargets.GetFlags().DefaultIfEmpty(AttributeTargets.All).Select(flag =>
        {
            CS.Syntax.ExpressionSyntax result;

            var flagName = CS.SyntaxFactory.IdentifierName(Enum.GetName(type, flag));
            var typeName = ParseTypeAsync(type);
            if (typeName is null)
                result = flagName;
            else
                result = CS.SyntaxFactory.MemberAccessExpression(
                    CS.SyntaxKind.SimpleMemberAccessExpression,
                    typeName,
                    CS.SyntaxFactory.IdentifierName(Enum.GetName(type, flag))
                );

            return result;
        }).ToArray();

        if (operands.Length == 1)
            return CS.SyntaxFactory.AttributeArgument(operands[0]);
        else
            return CS.SyntaxFactory.AttributeArgument(
                operands.Skip(1).Aggregate(
                    operands[0],
                    (left, right) => CS.SyntaxFactory.BinaryExpression(CS.SyntaxKind.BitwiseOrExpression, left, right)
                )
            );

        CS.Syntax.ExpressionSyntax? ParseTypeAsync(Type type)
        {
            var nameExpression = CS.SyntaxFactory.IdentifierName(type.Name);
            var fullNameExpression = CS.SyntaxFactory.ParseTypeName(type.FullName);
            var namespaceExpression = CS.SyntaxFactory.ParseTypeName(type.Namespace);
            foreach (var syntax in root.DescendantNodes().OfType<CS.Syntax.UsingDirectiveSyntax>().Concat(
                compilation.SyntaxTrees.SelectMany(tree =>
                    tree.GetRoot(cancellationToken).DescendantNodes().OfType<CS.Syntax.UsingDirectiveSyntax>()
                        .Where(@global => !global.GlobalKeyword.IsKind(CS.SyntaxKind.None))
            )))
            {
                if (syntax.StaticKeyword.IsKind(CS.SyntaxKind.None)) // 不是using static语句。
                {
                    if (syntax.Name.IsEquivalentTo(namespaceExpression)) // using命名空间。
                    {
                        if (syntax.Alias?.Name.Identifier is SyntaxToken aliasName && !aliasName.IsKind(CS.SyntaxKind.None)) // 给命名空间取别名。
                        {
                            return CS.SyntaxFactory.QualifiedName(
                                CS.SyntaxFactory.IdentifierName(aliasName.Text),
                                CS.SyntaxFactory.Token(CS.SyntaxKind.DotToken),
                                nameExpression
                            );
                        }

                        return nameExpression;
                    }
                    else if (syntax.Name.IsEquivalentTo(fullNameExpression)) // 给类型取别名。
                    {
                        // 此时语法正确时必定会存在类型别名。
                        if (syntax.Alias?.Name.Identifier is SyntaxToken aliasName && !aliasName.IsKind(CS.SyntaxKind.None))
                        {
                            return CS.SyntaxFactory.IdentifierName(aliasName.Text);
                        }
                        else
                        {
                            // 语法错误情况，忽略。
                        }
                    }
                }
                else // 是using static语句。
                {
                    if (syntax.Name.IsEquivalentTo(fullNameExpression))
                        return null;
                }
            }

            return fullNameExpression;
        }
    }

    private VB.Syntax.ArgumentSyntax CreateVisualBasicAttributeArgument(AttributeTargets expectedTargets, SyntaxNode root)
    {
        var type = typeof(AttributeTargets);
        var operands = expectedTargets.GetFlags().DefaultIfEmpty(AttributeTargets.All).Select(flag =>
        {
            VB.Syntax.ExpressionSyntax result;

            var flagName = VB.SyntaxFactory.IdentifierName(Enum.GetName(type, flag));
            var typeName = ParseTypeAsync(type);
            if (typeName is null)
                result = flagName;
            else
                result = VB.SyntaxFactory.SimpleMemberAccessExpression(
                    typeName,
                    VB.SyntaxFactory.Token(VB.SyntaxKind.DotToken),
                    VB.SyntaxFactory.IdentifierName(Enum.GetName(type, flag))
                );

            return result;
        }).ToArray();

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

        VB.Syntax.ExpressionSyntax? ParseTypeAsync(Type type)
        {
            var nameExpression = VB.SyntaxFactory.IdentifierName(type.Name);
            var fullNameExpression = VB.SyntaxFactory.ParseTypeName(type.FullName);
            var namespaceExpression = VB.SyntaxFactory.ParseTypeName(type.Namespace);
            foreach (var syntax in root.DescendantNodes().OfType<VB.Syntax.ImportsStatementSyntax>().SelectMany(@is => @is.ImportsClauses).OfType<VB.Syntax.SimpleImportsClauseSyntax>())
            {
                if (syntax.Name.IsEquivalentTo(nameExpression)) // Imports命名空间。
                {
                    if (syntax.Alias?.Identifier is SyntaxToken aliasName && !aliasName.IsKind(VB.SyntaxKind.None)) // 给命名空间取别名。
                    {
                        return VB.SyntaxFactory.QualifiedName(
                            VB.SyntaxFactory.IdentifierName(aliasName.Text),
                            VB.SyntaxFactory.Token(VB.SyntaxKind.DotToken),
                            nameExpression
                        );
                    }

                    return nameExpression;
                }
                else if (syntax.Name.IsEquivalentTo(fullNameExpression)) // Imports类型。
                {
                    if (syntax.Alias?.Identifier is SyntaxToken aliasName && !aliasName.IsKind(VB.SyntaxKind.None)) // 给类型取别名。
                    {
                        return VB.SyntaxFactory.IdentifierName(aliasName.Text);
                    }

                    return null;
                }
            }

            return fullNameExpression;
        }
    }
}
