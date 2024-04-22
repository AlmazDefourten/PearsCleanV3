using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.MatchesLists.Queries.GetMatches;

public record GetMatchesQuery : IRequest<MatchesVm>
{
    public int ListId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public long UserId { get; set; }
}

public class GetMatchesQueryValidator : AbstractValidator<GetMatchesQuery>
{
    public GetMatchesQueryValidator()
    {
    }
}

public class GetMatchesQueryHandler : IRequestHandler<GetMatchesQuery, MatchesVm>
{
    private readonly IApplicationDbContext _context;

    public GetMatchesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MatchesVm> Handle(GetMatchesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
