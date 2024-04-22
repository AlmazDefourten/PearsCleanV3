﻿using PearsCleanV3.Domain.Entities;

namespace PearsCleanV3.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }
    
    DbSet<Match> Matches { get; }
    
    DbSet<ApplicationUser> Users { get; }
    
    DbSet<Message> Messages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
