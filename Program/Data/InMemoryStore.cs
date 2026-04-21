using Program.Models;

namespace Program.Data;

public static class InMemoryStore
{
    private static readonly object SyncRoot = new();

    public static List<Room> Rooms { get; } = new();
    public static List<Reservation> Reservations { get; } = new();

    static InMemoryStore()
    {
        Rooms.AddRange(new[]
        {
            new Room { Id = 1, Name = "A-101", BuildingCode = "A", Floor = 1, Capacity = 30, HasProjector = true, IsActive = true },
            new Room { Id = 2, Name = "A-102", BuildingCode = "A", Floor = 1, Capacity = 18, HasProjector = false, IsActive = true },
            new Room { Id = 3, Name = "B-204", BuildingCode = "B", Floor = 2, Capacity = 24, HasProjector = true, IsActive = true },
            new Room { Id = 4, Name = "C-301", BuildingCode = "C", Floor = 3, Capacity = 40, HasProjector = true, IsActive = false },
            new Room { Id = 5, Name = "C-302", BuildingCode = "C", Floor = 3, Capacity = 12, HasProjector = false, IsActive = true }
        });

        Reservations.AddRange(new[]
        {
            new Reservation { Id = 1, RoomId = 1, OrganizerName = "Anna Kowalska", Topic = "Warsztaty z HTTP", Date = new DateOnly(2026, 5, 10), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 0), Status = "confirmed" },
            new Reservation { Id = 2, RoomId = 1, OrganizerName = "Piotr Nowak", Topic = "REST w praktyce", Date = new DateOnly(2026, 5, 10), StartTime = new TimeOnly(13, 0), EndTime = new TimeOnly(14, 30), Status = "planned" },
            new Reservation { Id = 3, RoomId = 2, OrganizerName = "Marta Zielińska", Topic = "Git dla początkujących", Date = new DateOnly(2026, 5, 11), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(11, 0), Status = "confirmed" },
            new Reservation { Id = 4, RoomId = 3, OrganizerName = "Jan Wiśniewski", Topic = "ASP.NET Core", Date = new DateOnly(2026, 5, 12), StartTime = new TimeOnly(15, 0), EndTime = new TimeOnly(17, 0), Status = "planned" },
            new Reservation { Id = 5, RoomId = 5, OrganizerName = "Ewa Kaczmarek", Topic = "Projektowanie API", Date = new DateOnly(2026, 5, 10), StartTime = new TimeOnly(8, 30), EndTime = new TimeOnly(9, 30), Status = "cancelled" },
            new Reservation { Id = 6, RoomId = 3, OrganizerName = "Adam Lewandowski", Topic = "Debugowanie w .NET", Date = new DateOnly(2026, 5, 14), StartTime = new TimeOnly(12, 0), EndTime = new TimeOnly(13, 30), Status = "confirmed" }
        });
    }

    public static int GetNextRoomId()
    {
        lock (SyncRoot)
        {
            return Rooms.Count == 0 ? 1 : Rooms.Max(room => room.Id) + 1;
        }
    }

    public static int GetNextReservationId()
    {
        lock (SyncRoot)
        {
            return Reservations.Count == 0 ? 1 : Reservations.Max(reservation => reservation.Id) + 1;
        }
    }
}

