namespace MassChecker;

/// <summary>
/// Attribute used to mark methods in classes derived from <see cref="MassCheckerBase{TItem, TFilter}"/>
/// as conditions for filtering. Only methods marked with this attribute are considered when evaluating
/// an item against the filter criteria.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class CheckerAttribute : Attribute
{
}