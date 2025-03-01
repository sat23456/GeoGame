using GeoGame.Models;
using GeoGame.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public LocationController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        // GET api/location
        [HttpGet]
        public ActionResult<List<Location>> Get()
        {
            var locations = _mongoDbService.GetAll<Location>("Location");
            return Ok(locations);
        }

        [HttpGet("{id}")]
        public ActionResult<Location> Get(string id)
        {
            var location = _mongoDbService.GetByFilter<Location>("Location", "cityId", id);
            if (location == null)
            {
                return NotFound();
            }
            return Ok(location);
        }

        // POST api/location
        [HttpPost]
        public ActionResult Create([FromBody] Location location)
        {
            _mongoDbService.Create("Location", location);
            return CreatedAtAction(nameof(Get), new { id = location.Id }, location);
        }

        // PUT api/location/{id}
        [HttpPut("{id}")]
        public ActionResult Update(string id, [FromBody] Location location)
        {
            _mongoDbService.Update("Location", id, location);
            return NoContent();
        }

        // DELETE api/location/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            _mongoDbService.Delete<Location>("Location", id);
            return NoContent();
        }
    }
}
