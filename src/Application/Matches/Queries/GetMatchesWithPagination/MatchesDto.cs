using PearsCleanV3.Application.Users.Queries;
using PearsCleanV3.Domain.Entities;

namespace PearsCleanV3.Application.Matches.Queries.GetMatchesWithPagination;

public class MatchesDto : UserDto
{
    public string? SwipedUserId { get; set; }
    
    public string? MatchedUserId { get; set; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Match, MatchesDto>();
        }
    }
}
