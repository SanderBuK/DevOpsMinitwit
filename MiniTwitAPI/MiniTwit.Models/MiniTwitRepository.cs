﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using MiniTwit.Entities;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace MiniTwit.Models
{
    public class MiniTwitRepository : IMiniTwitRepository
    {
        private readonly IMiniTwitContext _context;
        private User _currentUser;

        public MiniTwitRepository(IMiniTwitContext context)
        {
            _context = context;
        }

        public async Task<Message> GetMessage(int messageId)
        {
            var message = Task.Run(() => (from m in _context.Messages
                where m.MessageId == messageId
                select new Message
                {
                    AuthorId = m.AuthorId,
                    MessageId = m.MessageId,
                    PubDate = m.PubDate,
                    Flagged = m.Flagged,
                    Text = m.Text
                }).FirstOrDefault());
            return await message;
        }

        public async Task<IEnumerable<Message>> GetAuthorMessages(int authorId)
        {
            var messages = Task.Run(() => from m in _context.Messages
                where m.AuthorId == authorId
                select new Message
                {
                    AuthorId = m.AuthorId,
                    MessageId = m.MessageId,
                    PubDate = m.PubDate,
                    Flagged = m.Flagged,
                    Text = m.Text
                });
            return await messages;
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync()
        {
            var messages = Task.Run(() => (from m in _context.Messages
                select new Message
                {
                    AuthorId = m.AuthorId,
                    MessageId = m.MessageId,
                    PubDate = m.PubDate,
                    Flagged = m.Flagged,
                    Text = m.Text
                }).ToList());
            return await messages;
        }


        // User Methods TODO: Place in own repository
        public async Task<long> GetUserId(string username)
        {
            var user_id = await Task.Run(() => (from u in _context.Users
                where u.Username == username
                select u.UserId).FirstOrDefault());
            return user_id;
        }

        public async Task FollowUser(string username)
        {
            var WhomId = await GetUserId(username);

            var follower = new Follower
            {
                WhoId = 2,
                WhomId = WhomId
            };

            _context.Followers.Add(follower);
        }

        public async Task UnfollowUser(string username)
        {
            var WhomId = await GetUserId(username);

            var follower = (from f in _context.Followers
                where f.WhoId == 2 && f.WhomId == WhomId
                select f).FirstOrDefault();

            _context.Followers.Remove(follower);
        }

        //Displays the latest messages of all users, limited by per_page
        public async Task<IEnumerable<TimelineDTO>> PublicTimeline(int per_page)
        {
            var messages = await Task.Run(() => (from m in _context.Messages
                join u in _context.Users on m.AuthorId equals u.UserId
                where m.Flagged == 0
                orderby m.PubDate descending
                select new TimelineDTO
                {
                    message = m,
                    user = u
                }).Take(per_page).ToList());

            return messages;
        }

        public async Task<IEnumerable<TimelineDTO>> Timeline(int per_page, int? currentuser) //TODO:pls deletion of the currentuser
        {
            if (currentuser == null)
            {
                return await PublicTimeline(per_page);
            }

            var messages = await Task.Run(() => from m in _context.Messages
                join u in _context.Users on m.AuthorId equals u.UserId
                where m.Flagged == 0 && m.AuthorId == u.UserId && (
                    u.UserId == currentuser || _context.Followers
                        .Where(f => f.WhomId == currentuser)
                        .Select(f => f.WhoId)
                        .Contains(u.UserId)
                )
                orderby m.PubDate descending
                select new TimelineDTO
                {
                    message = m,
                    user = u
                });
            
            return messages.ToList().Take(per_page);
        }

        public async Task Login(string username, string password)
        {
            var user = await (from u in _context.Users
                                    where u.Username == username
                                    select u).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid username");
            }

            using SHA256 sha256 = SHA256.Create();
            
            
            //Compares sha256 byte array of input password to byte array of stored hash of password - if not the same, throws exception
             if (!sha256.ComputeHash(Encoding.ASCII.GetBytes(password)).SequenceEqual(Encoding.ASCII.GetBytes(user.PwHash.Split(":")[2])))
             {
                 throw new ArgumentException("Invalid password");
             }
            
            //  Do this for the testing
            // var userReturn = user;
            //
            // if (!Encoding.ASCII.GetBytes(password).SequenceEqual(Encoding.ASCII.GetBytes(user.PwHash.Split(":")[2])))
            // {
            //     throw new ArgumentException("Invalid password");
            // }
            //
            // if (Encoding.ASCII.GetBytes(password).SequenceEqual(Encoding.ASCII.GetBytes(user.PwHash.Split(":")[2])))
            // {
            //     userReturn = user;
            // }

            //return userReturn;
            _currentUser = user;
        }
    }
}