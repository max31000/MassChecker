class Program
{
    public static void Main(string[] args)
    {
        var filter = new RoflanFilter();
        var filterDto = new RoflanFilterDto
        {
            Number = 10,
        };

        var roflans = new[]
        {
            new RoflanDto
            {
                Number = 10,
                Name = "a",
            },
            new RoflanDto
            {
                Number = 4,
                Name = "ab",
            },
            new RoflanDto
            {
                Number = 10,
                Name = "abo",
            },
            new RoflanDto
            {
                Number = 1,
                Name = "abob",
            },
            new RoflanDto
            {
                Number = 10,
                Name = "aboba",
            },
        };

        var roflansAfterFilter = roflans.Where(x => filter.NeedItem(filterDto, x));
        Console.WriteLine(string.Join(", " , roflansAfterFilter.Select(x => x.Name)));
    }
}