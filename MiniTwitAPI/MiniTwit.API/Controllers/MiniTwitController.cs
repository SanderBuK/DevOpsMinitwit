using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MiniTwit.Entities;
using MiniTwit.Models;
using Prometheus;
using static Microsoft.AspNetCore.Http.StatusCodes;


namespace MiniTwit.API.Controllers
{
    public enum CacheFields
    {
        Latest
    }

    [ApiController]
    [Route("/")]
    public class MiniTwitController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private IMemoryCache _memoryCache;
        private static readonly Gauge TotalUsers =
            Metrics.CreateGauge("minitwit_total_users", "Number of unique users registered");
        private static readonly Gauge TotalFollowers =
            Metrics.CreateGauge("minitwit_total_followers", "Number of total follows");
        private static readonly Gauge AverageFollows =
            Metrics.CreateGauge("minitwit_average_follow", "Number of average follows by user");
        private static readonly Gauge AverageMessages =
            Metrics.CreateGauge("minitwit_average_messages", "Number of average messages by user");
        private static readonly Gauge TotalMessages =
            Metrics.CreateGauge("minitwit_total_messages", "Number of total messages");

        public MiniTwitController(
            IUserRepository userRepository, 
            IMessageRepository messageRepository,
            IMemoryCache memoryCache)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _memoryCache = memoryCache;
            //TotalUsers.IncTo(_userRepository.TotalUsers());
            //TotalFollowers.IncTo(_userRepository.TotalFollows());
            //AverageFollows.IncTo(_userRepository.AverageFollowsByUser());
            //AverageMessages.IncTo(_userRepository.AverageMessagesPostedByUser());
            //TotalMessages.IncTo(_userRepository.TotalMessages());
        }

        [HttpGet]
        public dynamic GetStatus()
        {
            return new { welcome_to = "MiniTwit API" };
        }

        [HttpGet("latest/")]
        public dynamic GetLatest()
        {
            return new { latest = _memoryCache.Get<int>(CacheFields.Latest) };
        }

        [HttpGet("msgs/")]
        public async Task<IEnumerable<TimelineDTO>> GetMessages(int no = 30, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            return await _userRepository.PublicTimeline(no);
        }

        [HttpGet("feed/")]
        public async Task<IEnumerable<TimelineDTO>> GetFeed(long userId, int no = 30, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            return await _userRepository.Timeline(no, (int) userId);
        }

        [HttpGet("msgs/{username}")]
        public async Task<IEnumerable<TimelineDTO>> GetUserMessages(string username, int no = 30, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            return await _messageRepository.GetUserMessages(username, no);
        }

        [HttpPost("msgs/{username}")]
        public async Task<IActionResult> PostUserMessages([FromBody] MessageCreateDTO request, string username, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            await _messageRepository.AddMessage(request, username);
            //AverageMessages.IncTo(_userRepository.AverageMessagesPostedByUser());
            //TotalMessages.Inc();
            return NoContent();
        }

        [HttpPost("fllws/{username}")]
        [ProducesResponseType(Status204NoContent)]
        public async Task<ActionResult> FollowUser([FromBody] FollowDTO request, string username, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            var response = HttpStatusCode.InternalServerError;
            if (request.follow != null)
            {
                response = await _userRepository.FollowUser(username, request.follow);
                //TotalFollowers.Inc();
            }
            else if(request.unfollow != null)
            {
                response = await _userRepository.UnfollowUser(username, request.unfollow);
                //TotalFollowers.Inc(-1);
            }
           
            //AverageFollows.IncTo(_userRepository.AverageFollowsByUser());
            
            return new StatusCodeResult((int)response);
        }

        [HttpPost("login/")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<User> Login([FromBody] LoginDTO dto, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);
            var user = await _userRepository.Login(dto.username, dto.password);
            if (user == null) return null;
            return user;
        }

        [HttpPost("register/")]
        public async Task<IActionResult> RegisterEndpoint([FromBody] RegisterDTO registration, int latest = 0)
        {
            _memoryCache.Set(CacheFields.Latest, latest);

            var user = await Register(registration);

            if (user == null) return BadRequest();
            
            //TotalUsers.Inc();
            return NoContent();
        }
        public async Task<User> Register(RegisterDTO registration)
        {
            var dto = new UserCreateDTO()
                        {
                            Username = registration.username,
                            Password = registration.pwd,
                            Email = registration.email
                        };

            var user = await _userRepository.RegisterUser(dto);

            return user;
        }

        public async Task<bool> IsFollowing(string follower, string follows)
        {
            return await _userRepository.IsFollowing(follower, follows);
        }
    }
}
