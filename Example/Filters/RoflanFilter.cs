// инициализируем через DI как синглтон

using Example.Models;
using MassChecker;

namespace Example.Filters;

public class RoflanFilter : MassCheckerBase<RoflanDto, RoflanFilterDto>
{
    [Checker]
    private bool CheckNumber(RoflanDto roflanDto, RoflanFilterDto filterDto)
    {
        return  true;
    }

    [Checker]
    private bool CheckName(RoflanDto roflanDto, RoflanFilterDto filterDto)
    {
        return filterDto.Name == null || roflanDto.Name == filterDto.Name;
    }

    [Checker]
    private bool CheckDate(RoflanDto roflanDto, RoflanFilterDto filterDto)
    {
        return (filterDto.DateFrom == null && filterDto.DateTo == null) 
               || roflanDto.Date > filterDto.DateFrom
               || roflanDto.Date < filterDto.DateTo;
    }
}