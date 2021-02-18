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

        [HttpGet("{username}/")]
        public async Task<IEnumerable<Message>> GetUserMessages(string username)
        {
            return await _repository.GetUserMessages(username);
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

        [HttpGet("login/")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<long?> Login([FromBody] LoginDTO dto)
        {
            var user = await _repository.Login(dto.username, dto.password);
            return user;
        }

        [HttpPost("register/")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterDTO registration)
        {
            var dto = new UserCreateDTO()
            {
                Username = registration.username,
                Password = registration.pwd,
                Email = registration.email
            };

            var userid = await _repository.RegisterUser(dto);
            return new StatusCodeResult((int) userid);
        }

        [HttpPost("/logout")]
        public void Logout()
        {
            _repository.Logout();
        }
    }
}
