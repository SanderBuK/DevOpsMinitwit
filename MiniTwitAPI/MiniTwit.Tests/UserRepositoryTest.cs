using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using MiniTwit.Entities;
using Xunit;
using Microsoft.EntityFrameworkCore.Sqlite;
using MiniTwit.Models;
using Microsoft.Data.Sqlite;

namespace Models.Test
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly MiniTwitContext context;
        private readonly UserRepository repo;

        public UserRepositoryTests()
        {
            var builder = new DbContextOptionsBuilder<MiniTwitContext>();
            builder.UseInMemoryDatabase(databaseName: "MiniTwitDatabase");  

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
        public async Task create_user_succesfully()
        {
            var result = await repo.RegisterUser(
                new UserCreateDTO
                {
                    Username = "userTest",
                    Email = "userTest@mail.com",
                    Password = "123"
                });

            var userQuery = from u in context.Users 
                            where u.Username == "userTest" 
                            select u;
            
            var user = await userQuery.FirstOrDefaultAsync();

            Assert.NotNull(user);
            Assert.Equal("userTest", user.Username);
            Assert.Equal("userTest@mail.com", user.Email);
            Assert.NotEqual("123", user.PwHash);
        }

        [Fact]
        public async Task follow_existing_user()
        {
            var followeduser = await repo.FollowUser("olduser2", "olduser1");
            var followinguser = await repo.FollowUser("olduser1", "olduser2");


            Assert.True(await repo.IsFollowing("olduser1", "olduser2"));
            Assert.True(await repo.IsFollowing("olduser2", "olduser1"));
        }

        [Fact]
        public async Task unfollow_existing_user_being_followed()
        {
            var followeduser = await repo.UnfollowUser("olduser2", "olduser1");
            var followinguser = await repo.UnfollowUser("olduser1", "olduser2");

            Assert.False(await repo.IsFollowing("olduser1", "olduser2"));
            Assert.False(await repo.IsFollowing("olduser2", "olduser1"));
        }

        [Fact]
        public async Task get_userId_by_username()
        {
            var userId1 = await repo.GetUserId("olduser1");
            var userId2 = await repo.GetUserId("olduser2");
            var userId3 = await repo.GetUserId("olduser3");
            var userId4 = await repo.GetUserId("olduser4");

            Assert.Equal(1, userId1);
            Assert.Equal(2, userId2);
            Assert.Equal(3, userId3);
            Assert.Equal(4, userId4);
        }

    }
}