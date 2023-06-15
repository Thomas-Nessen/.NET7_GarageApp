using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using GarageApp.DAL;
using GarageApp.Models;


namespace GarageApp.Controllers
{
    [EnableCors("AllowTheseOrigins")]
    [Route("[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private IWork _work;       // not sure why this is needed

        public DatabaseController(IWork work)
        {
            _work = work; // grab work interface from IWork
        }

        [HttpPost("/garage/parkyourcar")]
        public async Task<IActionResult> AddParkedCar(ParkedCarData dto)
        {
            return Ok(await _work.AddParkedCarAsync(dto));
        }

        [HttpGet("/signup")]
        public string HelloWorld(string? name)
        {
            if (name == null) return "Hello Empty World";

            return "Hello " + name;
        }



    }
}
