using System.Linq.Expressions;
using System.Reflection;

public abstract class MassCheckerBase<TItem, TFilter>
{
    protected MassCheckerBase()
    {
        var methods = GetType()
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        var checkerMethods = methods.Where(m => m.GetCustomAttributes(typeof(CheckerAttribute), true).Length > 0)
            .ToArray();

        Checkers = checkerMethods.Select(x => x.CreateDelegate(
                Expression.GetDelegateType(
                    x.GetParameters()
                        .Select(p => p.ParameterType)
                        .Union(new []{ x.ReturnType })
                        .ToArray()), 
                this))
            .Select(x => (Func<TItem, TFilter, bool>)x)
            .ToArray();
    }

    protected readonly Func<TItem, TFilter, bool>[] Checkers;
}