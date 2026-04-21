using Microsoft.AspNetCore.Mvc;
using TrainingCenterApi.Data;
using TrainingCenterApi.Models;

namespace TrainingCenterApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Reservation>> GetAll([FromQuery] DateOnly? date, [FromQuery] string? status, [FromQuery] int? roomId)
    {
        var reservations = InMemoryStore.Reservations.AsEnumerable();

        if (date.HasValue)
        {
            reservations = reservations.Where(reservation => reservation.Date == date);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            reservations = reservations.Where(reservation => string.Equals(reservation.Status, status, StringComparison.OrdinalIgnoreCase));
        }

        if (roomId.HasValue)
        {
            reservations = reservations.Where(reservation => reservation.RoomId == roomId.Value);
        }

        return Ok(reservations.ToList());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Reservation> GetById(int id)
    {
        var reservation = InMemoryStore.Reservations.FirstOrDefault(existing => existing.Id == id);
        return reservation is null ? NotFound() : Ok(reservation);
    }

    [HttpPost]
    public ActionResult<Reservation> Create(Reservation reservation)
    {
        var room = InMemoryStore.Rooms.FirstOrDefault(existing => existing.Id == reservation.RoomId);
        if (room is null)
        {
            return NotFound(new { message = "Room not found." });
        }

        if (!room.IsActive)
        {
            return Conflict(new { message = "Reservations cannot be created for inactive rooms." });
        }

        if (HasTimeConflict(reservation))
        {
            return Conflict(new { message = "Reservation conflicts with an existing reservation in the same room." });
        }

        var newReservation = new Reservation
        {
            Id = InMemoryStore.GetNextReservationId(),
            RoomId = reservation.RoomId,
            OrganizerName = reservation.OrganizerName,
            Topic = reservation.Topic,
            Date = reservation.Date,
            StartTime = reservation.StartTime,
            EndTime = reservation.EndTime,
            Status = reservation.Status
        };

        InMemoryStore.Reservations.Add(newReservation);

        return CreatedAtAction(nameof(GetById), new { id = newReservation.Id }, newReservation);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Reservation> Update(int id, Reservation reservation)
    {
        var existingReservation = InMemoryStore.Reservations.FirstOrDefault(existing => existing.Id == id);
        if (existingReservation is null)
        {
            return NotFound();
        }

        var room = InMemoryStore.Rooms.FirstOrDefault(existing => existing.Id == reservation.RoomId);
        if (room is null)
        {
            return NotFound(new { message = "Room not found." });
        }

        if (!room.IsActive)
        {
            return Conflict(new { message = "Reservations cannot be assigned to inactive rooms." });
        }

        if (HasTimeConflict(reservation, id))
        {
            return Conflict(new { message = "Reservation conflicts with an existing reservation in the same room." });
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

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var existingReservation = InMemoryStore.Reservations.FirstOrDefault(reservation => reservation.Id == id);
        if (existingReservation is null)
        {
            return NotFound();
        }

        InMemoryStore.Reservations.Remove(existingReservation);
        return NoContent();
    }

    private static bool HasTimeConflict(Reservation candidate, int? reservationIdToIgnore = null)
    {
        if (candidate.Status is not null && string.Equals(candidate.Status, "cancelled", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!candidate.Date.HasValue || !candidate.StartTime.HasValue || !candidate.EndTime.HasValue)
        {
            return false;
        }

        return InMemoryStore.Reservations.Any(existing =>
            existing.Id != reservationIdToIgnore &&
            existing.RoomId == candidate.RoomId &&
            !string.Equals(existing.Status, "cancelled", StringComparison.OrdinalIgnoreCase) &&
            existing.Date == candidate.Date &&
            existing.StartTime.HasValue &&
            existing.EndTime.HasValue &&
            candidate.StartTime.Value < existing.EndTime.Value &&
            candidate.EndTime.Value > existing.StartTime.Value);
    }
}

