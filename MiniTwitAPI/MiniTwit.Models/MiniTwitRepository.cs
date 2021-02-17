﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using MiniTwit.Entities;
using Microsoft.EntityFrameworkCore;
using static System.Net.HttpStatusCode;

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

        public async Task<long> AddMessage(MessageCreateDTO message, string username)
        {
            var userId = await GetUserId(username);

            var newMessage = new Message
            {
                AuthorId = userId,
                Text = message.content,
                PubDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Flagged = 0
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            return newMessage.MessageId;
        }

        public async Task<HttpStatusCode> DeleteMessage(long id)
        {
            var message = await _context.Messages.FindAsync(id);

            if(message == null)
            {
                return NotFound;
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent;
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

        public async Task<IEnumerable<Message>> GetUserMessages(string username, int per_page)
        {
            var userId = await GetUserId(username);

            var messages = await (from m in _context.Messages
                where m.AuthorId == userId
                select new Message
                {
                    AuthorId = m.AuthorId,
                    MessageId = m.MessageId,
                    PubDate = m.PubDate,
                    Flagged = m.Flagged,
                    Text = m.Text
                }).Take(per_page).ToListAsync();

            return messages;
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

        public async Task<HttpStatusCode> FollowUser(string username)
        {
            var WhomId = await GetUserId(username);

            var follower = new Follower
            {
                WhoId = _currentUser.UserId,
                WhomId = WhomId
            };

            _context.Followers.Add(follower);
            await _context.SaveChangesAsync();

            return OK;
        }

        public async Task<HttpStatusCode> UnfollowUser(string username)
        {
            var WhomId = await GetUserId(username);

            var follower = (from f in _context.Followers
                where f.WhoId == _currentUser.UserId && f.WhomId == WhomId
                select f).FirstOrDefault();

            _context.Followers.Remove(follower);
            await _context.SaveChangesAsync();

            return OK;
        }

        public async Task<IEnumerable<string>> GetFollowers()
        {
            var followers = await (from f in _context.Followers
                                   join u in _context.Users on f.WhomId equals u.UserId
                                   where f.WhoId == _currentUser.UserId
                                   select u.Username).ToListAsync();
            return followers;
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

        public async Task<IEnumerable<TimelineDTO>> Timeline(int per_page)
        {
            if (_currentUser == null)
            {
                return await PublicTimeline(per_page);
            }

            var messages = await Task.Run(() => (from m in _context.Messages
                join u in _context.Users on m.AuthorId equals u.UserId
                where m.Flagged == 0 && (
                    u.UserId == _currentUser.UserId || _context.Followers
                                                        .Where(f => f.WhoId == _currentUser.UserId)
                                                        .Select(f => f.WhomId)
                                                        .Contains(u.UserId)
                )
                orderby m.PubDate descending
                select new TimelineDTO
                {
                    message = m,
                    user = u
                }).Take(per_page).ToList());

            return messages;
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
            var storedHash = user.PwHash.Split("$");
            var salt = storedHash[1];
            var hashValue = storedHash[2];
            var computedHash = sha256.ComputeHash(Encoding.ASCII.GetBytes(salt+password));
            var computedHashAlt = sha256.ComputeHash(Encoding.ASCII.GetBytes(password+salt));
            
             if (!computedHash.SequenceEqual(Encoding.ASCII.GetBytes(hashValue)))
             {
                 throw new ArgumentException("Invalid password salt prefix");
             }
             
             if (!computedHashAlt.SequenceEqual(Encoding.ASCII.GetBytes(hashValue)))
             {
                 throw new ArgumentException("Invalid password salt suffix");
             }
             
            _currentUser = user;
        }
        public void Logout()
        {
            _currentUser = null;
        }
    }
}