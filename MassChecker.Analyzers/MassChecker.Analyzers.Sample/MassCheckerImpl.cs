using System;

namespace MassChecker.Analyzers.Sample;

public class MassCheckerImpl : MassCheckerBase<RoflanDto, RoflanFilterDto>
{
    [Checker]
    public bool IncorrectSignatureMethod(RoflanDto item, RoflanFilterDto filter)
    {
        return 1 > 2;
    }
}