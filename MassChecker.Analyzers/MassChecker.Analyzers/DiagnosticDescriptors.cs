using Microsoft.CodeAnalysis;

namespace MassChecker.Analyzers;

public static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor CheckerMethodSignatureIncorrect = new DiagnosticDescriptor(
        id: "MC001CheckerMethodSignature",
        title: "Method signature does not match the expected signature",
        messageFormat: "Method '{0}' with ConditionMethodAttribute does not have the expected signature (TItem, TFilter) -> bool",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "All methods marked with ConditionMethodAttribute must have two parameters of types TItem and TFilter and return a bool.");

    public static readonly DiagnosticDescriptor CheckerAttributeUsageError = new DiagnosticDescriptor(
        id: "MC002CheckerAttributeUsage",
        title: "Invalid Checker Attribute Usage",
        messageFormat: "The Checker attribute can only be applied to methods within classes derived from MassCheckerBase<TItem, TFilter>",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Ensure that the Checker attribute is only used on methods in classes inheriting from MassCheckerBase<TItem, TFilter>.");
}