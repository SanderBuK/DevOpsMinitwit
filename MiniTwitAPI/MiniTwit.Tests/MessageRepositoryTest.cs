using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Entities;
using Xunit;
using MiniTwit.Models;
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
                                                new MessageCreateDTO 
                                                    {
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

        [Fact]
        public async Task read_nonexisting_message()
        {
            var result = await repo.GetMessage(1337);

            Assert.Null(result);
        }
        
    }
}
