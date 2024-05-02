using System.Reflection;
using System.Runtime.Serialization;
using AutoMapper;
using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Domain.Entities;
using NUnit.Framework;
using PearsCleanV3.Application.Chats;
using PearsCleanV3.Application.Matches.Queries.GetMatchesWithPagination;
using PearsCleanV3.Application.Users.Queries;

namespace PearsCleanV3.Application.UnitTests.Common.Mappings;

public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        _configuration = new MapperConfiguration(config => 
            config.AddMaps(Assembly.GetAssembly(typeof(IApplicationDbContext))));

        _mapper = _configuration.CreateMapper();
    }

    [Test]
    [TestCase(typeof(ApplicationUser), typeof(UserDto))]
    [TestCase(typeof(Match), typeof(MatchesDto))]
    [TestCase(typeof(Message), typeof(MessageDto))]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        var instance = GetInstanceOf(source);

        _mapper.Map(instance, source, destination);
    }

    private object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;

#pragma warning disable SYSLIB0050
        return FormatterServices.GetUninitializedObject(type);
#pragma warning restore SYSLIB0050
    }
}
