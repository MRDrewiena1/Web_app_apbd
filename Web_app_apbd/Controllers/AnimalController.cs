using Microsoft.AspNetCore.Mvc;

namespace Web_app_apbd.Controllers
{
    // api/animal
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        public static List<Animal> Animals = new List<Animal>()
        {
            new Animal(){id = 1, name = "Fish", age = 20},
            new Animal(){id = 2 , name = "Pig", age = 30},
            new Animal(){id = 3, name = "Dog", age = 40}
        };
        
        [HttpPost]
        public IActionResult Post(int id,string name,int age)
        {
            Animals.Add(new Animal(){id = id, name = name, age = age});
            return Ok();
        }
        [HttpGet]
        public IActionResult Get(int? id)
        {
            foreach (var animal in Animals)
            {
                if (animal.id == id)
                {
                    return Ok(animal);
                }
            }
            return NoContent();
        }
    }
}
