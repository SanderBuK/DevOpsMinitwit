﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniTwit.Entities;
using MiniTwit.Models;
using static Microsoft.AspNetCore.Http.StatusCodes;


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

        [HttpPost("messages/")]
        public async Task<ActionResult<long>> AddMessage([FromBody] MessageCreateDTO message)
        {
            return await _repository.AddMessage(message);
        }

        [HttpDelete("messages/{id}")]
        public async Task<ActionResult> DeleteMessage(long id)
        {
            var response = await _repository.DeleteMessage(id);

            return new StatusCodeResult((int)response);
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

        [HttpGet("timeline/")]
        public async Task<IEnumerable<TimelineDTO>> GetTimeline() 
        {
            return await _repository.Timeline(30);
        }

        [HttpGet("followers/")]
        public async Task<IEnumerable<string>> GetFollowerts()
        {
            return await _repository.GetFollowers();
        }

        [HttpGet("login/username={username}+password={password}")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<HttpStatusCode> Login(string username, string password)
        {
            try
            {
                await _repository.Login(username, password);
                return HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                return HttpStatusCode.NotFound;
            }
        }
    }
}
