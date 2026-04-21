using Microsoft.AspNetCore.Mvc;
using TrainingCenterApi.Data;
using TrainingCenterApi.Models;

namespace TrainingCenterApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Room>> GetAll([FromQuery] int? minCapacity, [FromQuery] bool? hasProjector, [FromQuery] bool? activeOnly)
    {
        var rooms = InMemoryStore.Rooms.AsEnumerable();

        if (minCapacity.HasValue)
        {
            rooms = rooms.Where(room => room.Capacity >= minCapacity.Value);
        }

        if (hasProjector.HasValue)
        {
            rooms = rooms.Where(room => room.HasProjector == hasProjector.Value);
        }

        if (activeOnly == true)
        {
            rooms = rooms.Where(room => room.IsActive);
        }

        return Ok(rooms.ToList());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Room> GetById(int id)
    {
        var room = InMemoryStore.Rooms.FirstOrDefault(room => room.Id == id);
        return room is null ? NotFound() : Ok(room);
    }

    [HttpGet("building/{buildingCode}")]
    public ActionResult<IEnumerable<Room>> GetByBuilding(string buildingCode)
    {
        var rooms = InMemoryStore.Rooms
            .Where(room => string.Equals(room.BuildingCode, buildingCode, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(rooms);
    }

    [HttpPost]
    public ActionResult<Room> Create(Room room)
    {
        var newRoom = new Room
        {
            Id = InMemoryStore.GetNextRoomId(),
            Name = room.Name,
            BuildingCode = room.BuildingCode,
            Floor = room.Floor,
            Capacity = room.Capacity,
            HasProjector = room.HasProjector,
            IsActive = room.IsActive
        };

        InMemoryStore.Rooms.Add(newRoom);

        return CreatedAtAction(nameof(GetById), new { id = newRoom.Id }, newRoom);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Room> Update(int id, Room room)
    {
        var existingRoom = InMemoryStore.Rooms.FirstOrDefault(existing => existing.Id == id);
        if (existingRoom is null)
        {
            return NotFound();
        }

        existingRoom.Name = room.Name;
        existingRoom.BuildingCode = room.BuildingCode;
        existingRoom.Floor = room.Floor;
        existingRoom.Capacity = room.Capacity;
        existingRoom.HasProjector = room.HasProjector;
        existingRoom.IsActive = room.IsActive;

        return Ok(existingRoom);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var existingRoom = InMemoryStore.Rooms.FirstOrDefault(room => room.Id == id);
        if (existingRoom is null)
        {
            return NotFound();
        }

        var hasRelatedReservations = InMemoryStore.Reservations.Any(reservation =>
            reservation.RoomId == id && !string.Equals(reservation.Status, "cancelled", StringComparison.OrdinalIgnoreCase));

        if (hasRelatedReservations)
        {
            return Conflict(new { message = "Room cannot be deleted because it has related reservations." });
        }

        InMemoryStore.Rooms.Remove(existingRoom);
        return NoContent();
    }
}

