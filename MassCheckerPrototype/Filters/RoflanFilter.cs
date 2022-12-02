// инициализируем через DI как синглтон
public class RoflanFilter : MassCheckerBase<RoflanDto, RoflanFilterDto>
{
    public bool NeedItem(RoflanFilterDto filterDto, RoflanDto roflanDto)
    {
        return Checkers.All(x => x(roflanDto, filterDto));
    }

    [Checker]
    private bool CheckNumber(RoflanDto roflanDto, RoflanFilterDto filterDto)
    {
        return filterDto.Number == null || roflanDto.Number == filterDto.Number;
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