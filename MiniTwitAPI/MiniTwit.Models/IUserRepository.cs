﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MiniTwit.Entities;

namespace MiniTwit.Models
{
    public interface IUserRepository
    {
        Task<IEnumerable<TimelineDTO>> PublicTimeline(int per_page);
        Task<IEnumerable<TimelineDTO>> Timeline(int per_page);
        Task<long> GetUserId(string username);
        Task<HttpStatusCode> FollowUser(string username, string followName);
        Task<HttpStatusCode> UnfollowUser(string username, string unfollowName);
        Task<long?> Login(string username, string password);
        Task<long> RegisterUser(UserCreateDTO user);
        string GenerateHash(string password);
        Task<IEnumerable<string>> GetFollowers();
        void Logout();
    }
}