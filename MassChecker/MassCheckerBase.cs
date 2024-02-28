using System.Linq.Expressions;
using System.Reflection;

namespace MassChecker;

/// <summary>
/// Serves as the base class for creating filters with multiple conditions.
/// This generic class allows defining filters based on specified item and filter types.
/// </summary>
/// <typeparam name="TItem">The type of the items to be filtered.</typeparam>
/// <typeparam name="TFilter">The type of the filter to apply to the items.</typeparam>
public abstract class MassCheckerBase<TItem, TFilter>
{
    private readonly Func<TItem, TFilter, bool>[] checkers;

    protected MassCheckerBase()
    {
        var methods = GetType()
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        var checkerMethods = methods.Where(m => m.GetCustomAttributes(typeof(CheckerAttribute), true).Length > 0)
            .ToArray();

        checkers = checkerMethods.Select(x =>
            {
                var parameters = x.GetParameters();

                if (parameters.Length != 2 ||
                    parameters[0].ParameterType != typeof(TItem) ||
                    parameters[1].ParameterType != typeof(TFilter) ||
                    x.ReturnType != typeof(bool))
                    throw new InvalidOperationException(
                        $"The method {x.Name} has an incorrect signature. Expected a method with the signature (TItem, TFilter) -> bool.");

                return x.CreateDelegate(
                    Expression.GetDelegateType(
                        parameters.Select(p => p.ParameterType)
                            .Union(new[] { x.ReturnType })
                            .ToArray()),
                    this);
            })
            .Select(x => (Func<TItem, TFilter, bool>)x)
            .ToArray();
    }

    /// <summary>
    /// An internal array of delegate checkers, each corresponding to a method annotated with the <see cref="CheckerAttribute"/>.
    /// These delegates are used to evaluate the filter conditions on the items.
    /// </summary>
    internal IEnumerable<Func<TItem, TFilter, bool>> Checkers => checkers;

    /// <summary>
    /// Evaluates the item against all filter conditions defined in the derived class.
    /// </summary>
    /// <param name="itemDto">The item to evaluate.</param>
    /// <param name="filterDto">The filter criteria to apply to the item.</param>
    /// <returns>true if the item matches all filter criteria; otherwise, false.</returns>
    public virtual bool NeedItem(TItem itemDto, TFilter filterDto)
    {
        return checkers.All(predicate => predicate(itemDto, filterDto));
    }
}