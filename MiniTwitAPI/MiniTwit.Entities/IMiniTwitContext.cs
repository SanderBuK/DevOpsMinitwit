﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MiniTwit.Entities
{
    public interface IMiniTwitContext
    {
        DbSet<Follower> Followers {get; set;}
        DbSet<Message> Messages {get; set;}
        DbSet<User> Users {get; set;}
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}