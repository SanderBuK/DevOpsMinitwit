﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniTwit.Entities;
using MiniTwit.Models;

namespace MiniTwit.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MiniTwitController
    {
        private readonly IMiniTwitRepository _repository;

        public MiniTwitController(IMiniTwitRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("messages/{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            return await _repository.GetMessage(id);
        }

        [HttpGet("messages/")]
        public async Task<IEnumerable<Message>> GetMessages()
        {
            return await _repository.GetMessagesAsync();
        }

        [HttpGet("messages/user/{userid}")]
        public async Task<IEnumerable<Message>> GetAuthorMessages(int userid)
        {
            return await _repository.GetAuthorMessages(userid);
        }

        [HttpGet("users/{username}")]
        public async Task<ActionResult<long>> GetUser(string username)
        {
            return await _repository.GetUserId(username);
        }

        [HttpGet("timeline/{userid}")]
        public async Task<IEnumerable<TimelineDTO>> GetTimeline(int? userid)
        {
            return await _repository.Timeline(30, userid);
        }

        [HttpGet("login/username={username}+password={password}")]
        public async Task<User> Login(string username, string password)
        {
            return await _repository.Login(username, password);
        }
    }
}
