using System.Linq.Expressions;
using System.Reflection;

namespace MassChecker;

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

    internal IEnumerable<Func<TItem, TFilter, bool>> Checkers => checkers;

    public virtual bool NeedItem(TItem itemDto, TFilter filterDto)
    {
        return checkers.All(predicate => predicate(itemDto, filterDto));
    }
}