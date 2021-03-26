using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Entities;
using Xunit;
using Microsoft.EntityFrameworkCore.Sqlite;
using MiniTwit.Models;
using Microsoft.Data.Sqlite;
using Models.Test;

namespace MiniTwit.Tests
{
    public class MessageRepositoryTest : IDisposable
    {
        private readonly MiniTwitContext context;
        private readonly UserRepository repo;

        public MessageRepositoryTest()
        {
            var builder = new DbContextOptionsBuilder<MiniTwitContext>();
            builder.UseInMemoryDatabase(databaseName: "MiniTwitDatabase");   //.UseSqlite("datasource=:memory:");

            var dbContextOptions = builder.Options;
            context = new ContextTest(dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            repo = new UserRepository(context);
        }

        public void Dispose()
        {
            context.Dispose();
        }

        [Fact]
        public async Task unfollow_existing_user_being_followed()
        {
            var followeduser = await repo.UnfollowUser("olduser2", "olduser1");
            var followinguser = await repo.UnfollowUser("olduser1", "olduser2");

            //Assert.Equal(0, result);
            Assert.False(await repo.IsFollowing("olduser1", "olduser2"));
            Assert.False(await repo.IsFollowing("olduser2", "olduser1"));
        }

    }
}
