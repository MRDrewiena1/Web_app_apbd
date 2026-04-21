using Microsoft.AspNetCore.Mvc;
using Web_app_apbd.Models;

namespace Web_app_apbd.Controllers
{
    // api/animal
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        public static List<Reservation> Reservations = new List<Reservation>()
        {
            new Reservation()
            {
                Id = 1,
                RoomId = 1,
                OrganizerName = "Anna Kowalska",
                Topic = "APBD",
                Date = new DateOnly(2026, 5, 10),
                StartTime = new TimeOnly(10, 0, 0),
                EndTime = new TimeOnly(12, 30, 0),
                Status = "confirmed"
            },
            new Reservation()
            {
                Id = 2,
                RoomId = 2,
                OrganizerName = "Piotr Nowak",
                Topic = "PRI",
                Date = new DateOnly(2026, 5, 10),
                StartTime = new TimeOnly(14, 0, 0),
                EndTime = new TimeOnly(16, 30, 0),
                Status = "confirmed"
            },
            new Reservation()
            {
                Id = 3,
                RoomId = 3,
                OrganizerName = "Maria Lewandowska",
                Topic = "PPY",
                Date = new DateOnly(2026, 5, 12),
                StartTime = new TimeOnly(9, 0, 0),
                EndTime = new TimeOnly(11, 0, 0),
                Status = "planned"
            },
            new Reservation()
            {
                Id = 4,
                RoomId = 1,
                OrganizerName = "Jan Wiśniewski",
                Topic = "Bazy danych",
                Date = new DateOnly(2026, 5, 15),
                StartTime = new TimeOnly(13, 0, 0),
                EndTime = new TimeOnly(15, 0, 0),
                Status = "confirmed"
            },
            new Reservation()
            {
                Id = 5,
                RoomId = 2,
                OrganizerName = "Katarzyna Kowalski",
                Topic = "TPO",
                Date = new DateOnly(2026, 5, 11),
                StartTime = new TimeOnly(10, 0, 0),
                EndTime = new TimeOnly(13, 0, 0),
                Status = "cancelled"
            },
            new Reservation()
            {
                Id = 6,
                RoomId = 5,
                OrganizerName = "Tomasz Nowicki",
                Topic = "PCO",
                Date = new DateOnly(2026, 5, 20),
                StartTime = new TimeOnly(9, 0, 0),
                EndTime = new TimeOnly(17, 0, 0),
                Status = "planned"
            }
        };
        
        // GET: api/reservations
        [HttpGet]
        public IActionResult GetAllReservations()
        {
            return Ok(Reservations);
        }
 
        // GET: api/reservations/{id}
        [HttpGet("{id}")]
        public IActionResult GetReservationById(int id)
        {
            var reservation = Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
            {
                return NotFound(new { message = $"Rezerwacja z ID {id} nie została znaleziona" });
            }
            return Ok(reservation);
        }
 
        // GET: api/reservations?date=2026-05-10&status=confirmed&roomId=2
        [HttpGet]
        public IActionResult GetReservationsFiltered([FromQuery] DateOnly? date, [FromQuery] string status, [FromQuery] int? roomId)
        {
            var result = Reservations.AsEnumerable();
 
            if (date.HasValue)
            {
                result = result.Where(r => r.Date == date);
            }
 
            if (!string.IsNullOrEmpty(status))
            {
                result = result.Where(r => r.Status == status);
            }
 
            if (roomId.HasValue)
            {
                result = result.Where(r => r.RoomId == roomId.Value);
            }
 
            return Ok(result.ToList());
        }
 
        // POST: api/reservations
        [HttpPost]
        public IActionResult CreateReservation([FromBody] Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
 
            var room = RoomsController.Rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
            if (room == null)
            {
                return BadRequest(new { message = $"Sala z ID {reservation.RoomId} nie istnieje" });
            }
 
            if (!room.IsActive)
            {
                return BadRequest(new { message = "Nie można zarezerwować nieaktywnej sali" });
            }
 
            var conflictingReservations = Reservations
                .Where(r => r.RoomId == reservation.RoomId 
                    && r.Date == reservation.Date
                    && r.Status != "cancelled"
                    && !(r.EndTime <= reservation.StartTime || r.StartTime >= reservation.EndTime))
                .ToList();
 
            if (conflictingReservations.Count > 0)
            {
                return Conflict(new { message = "Istnieje już rezerwacja kolidująca czasowo dla tej sali w wybranym dniu" });
            }
 
            reservation.Id = Reservations.Count > 0 ? Reservations.Max(r => r.Id) + 1 : 1;
            Reservations.Add(reservation);
 
            return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);
        }
 
        // PUT: api/reservations/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateReservation(int id, [FromBody] Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
 
            var existingReservation = Reservations.FirstOrDefault(r => r.Id == id);
            if (existingReservation == null)
            {
                return NotFound(new { message = $"Rezerwacja z ID {id} nie została znaleziona" });
            }
 
            if (existingReservation.RoomId != reservation.RoomId)
            {
                var room = RoomsController.Rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
                if (room == null)
                {
                    return BadRequest(new { message = $"Sala z ID {reservation.RoomId} nie istnieje" });
                }
 
                if (!room.IsActive)
                {
                    return BadRequest(new { message = "Nie można zarezerwować nieaktywnej sali" });
                }
            }
 
            var conflictingReservations = Reservations
                .Where(r => r.Id != id
                    && r.RoomId == reservation.RoomId 
                    && r.Date == reservation.Date
                    && r.Status != "cancelled"
                    && !(r.EndTime <= reservation.StartTime || r.StartTime >= reservation.EndTime))
                .ToList();
 
            if (conflictingReservations.Count > 0)
            {
                return Conflict(new { message = "Istnieje już rezerwacja kolidująca czasowo dla tej sali w wybranym dniu" });
            }
 
            existingReservation.RoomId = reservation.RoomId;
            existingReservation.OrganizerName = reservation.OrganizerName;
            existingReservation.Topic = reservation.Topic;
            existingReservation.Date = reservation.Date;
            existingReservation.StartTime = reservation.StartTime;
            existingReservation.EndTime = reservation.EndTime;
            existingReservation.Status = reservation.Status;
 
            return Ok(existingReservation);
        }
 
        // DELETE: api/reservations/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteReservation(int id)
        {
            var reservation = Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
            {
                return NotFound(new { message = $"Rezerwacja z ID {id} nie została znaleziona" });
            }
 
            Reservations.Remove(reservation);
            return NoContent();
        }
    }
}
