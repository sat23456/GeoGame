using GeoGame.Models;
using GeoGame.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace GeoGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public UserController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }


        [HttpGet("{id}")]
        public ActionResult<FrontUser> Get(string id)
        {
            var user = _mongoDbService.GetByFilter<User>("Users", "UserId", id);
            if (user == null)
            {
                return NotFound();
            }
            FrontUser frontUser = new FrontUser()
            {
                Id = user.Id.ToString(),
                UserId = user.UserId,
                Name = user.Name,
                Cities = user.Cities,
                CurrentCityId = user.CurrentCityId,
                Friends = user.Friends,
                Score = user.Score
            };
            return Ok(frontUser);
        }

        [HttpPatch("update/{userId}")]
        public async Task<ActionResult> UpdateUser(string userId, [FromBody] FrontUser user)
        {
            // Call the MongoDbService method to update the user
            User updatedUser = new User()
            {
                Id = new ObjectId(user.Id),
                UserId = user.UserId,
                Name = user.Name,
                Cities = user.Cities,
                CurrentCityId = user.CurrentCityId,
                Friends = user.Friends,
                Score = user.Score
            };
            _mongoDbService.Update<User>("Users", "UserId", userId, updatedUser);
            return Ok(); // Return the error message
        }

        [HttpPost("moveToNextCity")]
        public async Task<ActionResult> MoveToNextCity([FromQuery] string userId)
        {
            // Call the MongoDbService method to move to the next city
            var result = await _mongoDbService.MoveToNextCityAsync(userId);

            if (result)
            {
                return Ok(result); // Successfully moved to the next city
            }

            return BadRequest(result); // Return the error message
        }

        [HttpPost("resetCityList")]
        public async Task<ActionResult> ResetCityList([FromQuery] string userId)
        {
            // Call the MongoDbService method to reset the city list
            var result = await _mongoDbService.ResetCityListAsync(userId);

            if (result)
            {
                return Ok(result); // Successfully reset the city list
            }

            return BadRequest(result); // Return the error message
        }

        [HttpPost("createFriendRequest")]
        public async Task<ActionResult> CreateFriendRequest([FromQuery] string userId)
        {
            // Call the MongoDbService method to create the friend request and generate the link
            var friendRequestLink = await _mongoDbService.CreateFriendRequestAsync(userId);

            // Return the generated friend request link
            return Ok(new { link = friendRequestLink });
        }

        // GET api/user/acceptFriendRequest
        [HttpGet("acceptFriendRequest")]
        public async Task<ActionResult> AcceptFriendRequest([FromQuery] string token, [FromQuery] string userId)
        {
            // Call the MongoDbService method to add the friend and handle the link acceptance
            var result = await _mongoDbService.AddFriendAsync(userId, token);

            if (result == "Friendship established!")
            {
                return Ok(result); // Successfully added both users to each other's friend list
            }

            return BadRequest(result); // Return the error message
        }
    }
}
