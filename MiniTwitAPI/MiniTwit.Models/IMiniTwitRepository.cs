﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using MiniTwit.Entities;

namespace MiniTwit.Models
{
    public interface IMiniTwitRepository
    {
        Task<Message> GetMessage(int id);
        Task<IEnumerable<Message>> GetMessagesAsync();
        Task<long> AddMessage(MessageCreateDTO message, string username);
        Task<HttpStatusCode> DeleteMessage(long id);
        Task<IEnumerable<Message>> GetUserMessages(string username, int per_page);
        Task<IEnumerable<TimelineDTO>> PublicTimeline(int per_page);
        Task<IEnumerable<TimelineDTO>> Timeline(int per_page);
        Task<long> GetUserId(string username);
        Task<HttpStatusCode> FollowUser(string username);
        Task<HttpStatusCode> UnfollowUser(string username);
        Task<IEnumerable<string>> GetFollowers();
        Task Login(string username, string password);
        void Logout();
    }
}
