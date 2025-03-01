using GeoGame.Models;
using GeoGame.Services;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<User> Get(string id)
        {
            var user = _mongoDbService.GetByFilter<User>("User", "userId", id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}
