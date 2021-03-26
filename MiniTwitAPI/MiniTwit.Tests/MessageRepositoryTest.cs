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
        private readonly MessageRepository repo;

        public MessageRepositoryTest()
        {
            var builder = new DbContextOptionsBuilder<MiniTwitContext>();
            builder.UseInMemoryDatabase(databaseName: "MiniTwitDatabase");

            var dbContextOptions = builder.Options;
            context = new ContextTest(dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            repo = new MessageRepository(context);
        }

        public void Dispose()
        {
            context.Dispose();
        }

        [Fact]
        public async Task create_and_message_existing_user_get_id()
        {
            var result = await repo.AddMessage(
                                                new MessageCreateDTO {
                                                    content = "this is a new post"
                                                }, 
                                                "olduser1");
            int messageId = 1;
         
            Assert.Equal(result, messageId);
        }

        [Fact]
        public async Task create_and_get_added_message()
        {
            var result = await repo.AddMessage(
                                                new MessageCreateDTO
                                                {
                                                    content = "this is a new post"
                                                },
                                                "olduser1");
            int messageId = 1;
            var getMessage = await repo.GetMessage(messageId);

            Assert.Equal(result, messageId);
            Assert.Equal(messageId, getMessage.MessageId);
        }

        /*
        [Fact]
        public async Task create_message_non_existing_user()
        {
            var result = await repo.CreateAsync("testtext", "nonuser");

            Assert.Equal(-1, result);
        }

        [Fact]
        public async Task read_nonexisting_message()
        {
            var result = await repo.ReadAsync(1337);

            Assert.Null(result);
        }

        [Fact]
        public async Task delete_existing_message()
        {
            var messageId = await repo.CreateAsync("testtext", "olduser2");
            var messagebefore = await repo.ReadAsync(messageId);
            var result = await repo.DeleteAsync(messageId);
            var messageafter = await repo.ReadAsync(messageId);

            Assert.NotNull(messagebefore);
            Assert.Null(messageafter);
            Assert.Equal(messageId, result);
        }

        [Fact]
        public async Task delete_nonexisting_message()
        {
            var result = await repo.DeleteAsync(1337);

            Assert.Equal(-1, result);
        }

        [Fact]
        public async Task readall_messages()
        {
            await repo.CreateAsync("testtext1", "olduser1");
            var first = await repo.ReadAllAsync();
            await repo.CreateAsync("testtext2", "olduser1");
            var second = await repo.ReadAllAsync();

            Assert.Equal(2, first.Count);
            Assert.Equal(3, second.Count);
        }
        */
    }
}
