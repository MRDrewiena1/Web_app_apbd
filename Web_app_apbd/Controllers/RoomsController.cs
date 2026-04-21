using Microsoft.AspNetCore.Mvc;
using Web_app_apbd.Controllers;
using Web_app_apbd.Models;


[Route("api/[controller]")]
[ApiController]
public class RoomsController : ControllerBase
{
    public static List<Room> Rooms = new List<Room>()
    {
        new Room()
        {
            Id = 1,
            Name = "Lab 101",
            BuildingCode = "A",
            Floor = 1,
            Capacity = 20,
            HasProjector = true,
            IsActive = true
        },
        new Room()
        {
            Id = 2,
            Name = "Lab 204",
            BuildingCode = "B",
            Floor = 2,
            Capacity = 24,
            HasProjector = true,
            IsActive = true
        },
        new Room()
        {
            Id = 3,
            Name = "Sala konferencyjna",
            BuildingCode = "B",
            Floor = 1,
            Capacity = 50,
            HasProjector = false,
            IsActive = true
        },
        new Room()
        {
            Id = 4,
            Name = "Pracownia 301",
            BuildingCode = "C",
            Floor = 3,
            Capacity = 15,
            HasProjector = true,
            IsActive = false
        },
        new Room()
        {
            Id = 5,
            Name = "Aula",
            BuildingCode = "A",
            Floor = 0,
            Capacity = 100,
            HasProjector = true,
            IsActive = true
        }
    };
    
        // GET: api/rooms
        [HttpGet]
        public IActionResult GetAllRooms([FromQuery] int? minCapacity, [FromQuery] bool? hasProjector, [FromQuery] bool? activeOnly)
        {
            var result = Rooms.AsEnumerable();
    
            if (minCapacity.HasValue)
                result = result.Where(r => r.Capacity >= minCapacity.Value);
    
            if (hasProjector.HasValue)
                result = result.Where(r => r.HasProjector == hasProjector.Value);
    
            if (activeOnly.HasValue && activeOnly.Value)
                result = result.Where(r => r.IsActive);
    
            return Ok(result.ToList());
        }
 
        // GET: api/rooms/{id}
        [HttpGet("{id}")]
        public IActionResult GetRoomById(int id)
        {
            var room = Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null)
            {
                return NotFound(new { message = $"Sala z ID {id} nie została znaleziona" });
            }
            return Ok(room);
        }
 
        // GET: api/rooms/building/{buildingCode}
        [HttpGet("building/{buildingCode}")]
        public IActionResult GetRoomsByBuilding(string buildingCode)
        {
            var rooms = Rooms.Where(r => r.BuildingCode == buildingCode).ToList();
            if (rooms.Count == 0)
            {
                return NotFound(new { message = $"Brak sal w budynku {buildingCode}" });
            }
            return Ok(rooms);
        }
        
        // POST: api/rooms
        [HttpPost]
        public IActionResult CreateRoom([FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
 
            room.Id = Rooms.Count > 0 ? Rooms.Max(r => r.Id) + 1 : 1;
            Rooms.Add(room);
 
            return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, room);
        }
 
        // PUT: api/rooms/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateRoom(int id, [FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
 
            var existingRoom = Rooms.FirstOrDefault(r => r.Id == id);
            if (existingRoom == null)
            {
                return NotFound(new { message = $"Sala z ID {id} nie została znaleziona" });
            }
 
            existingRoom.Name = room.Name;
            existingRoom.BuildingCode = room.BuildingCode;
            existingRoom.Floor = room.Floor;
            existingRoom.Capacity = room.Capacity;
            existingRoom.HasProjector = room.HasProjector;
            existingRoom.IsActive = room.IsActive;
 
            return Ok(existingRoom);
        }
 
        // DELETE: api/rooms/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteRoom(int id)
        {
            var room = Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null)
            {
                return NotFound(new { message = $"Sala z ID {id} nie została znaleziona" });
            }
 
            // Sprawdzenie, czy istnieją rezerwacje dla tej sali
            var reservationsForRoom = ReservationsController.Reservations
                .Where(res => res.RoomId == id && res.Status != "cancelled")
                .ToList();
 
            if (reservationsForRoom.Count > 0)
            {
                return Conflict(new { message = "Nie można usunąć sali, która posiada aktywne rezerwacje" });
            }
 
            Rooms.Remove(room);
            return NoContent();
        }
}