using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Matches.Commands.CreateMatch;

public record CreateMatchCommand : IRequest<bool>
{

}

public class CreateMatchCommandValidator : AbstractValidator<CreateMatchCommand>
{
    public CreateMatchCommandValidator()
    {
    }
}

public class CreateMatchCommandHandler : IRequestHandler<CreateMatchCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CreateMatchCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
