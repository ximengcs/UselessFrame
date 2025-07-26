using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "ASYNC001";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        id: DiagnosticId,
        title: "Async 方法缺少 Async 后缀",
        messageFormat: "方法 '{0}' 是异步方法但缺少 Async 后缀",
        category: "命名规范",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        // 检查 async 关键字
        if (method.Modifiers.Any(m => m.IsKind(SyntaxKind.AsyncKeyword)))
        {
            var methodName = method.Identifier.Text;
            if (!methodName.EndsWith("Async"))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    method.Identifier.GetLocation(),
                    methodName);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}