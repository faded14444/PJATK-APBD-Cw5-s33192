# Program API

Prosta aplikacja ASP.NET Core Web API oparta na kontrolerach.

## Endpointy

### Rooms
- `GET /api/rooms`
- `GET /api/rooms/{id}`
- `GET /api/rooms/building/{buildingCode}`
- `GET /api/rooms?minCapacity=20&hasProjector=true&activeOnly=true`
- `POST /api/rooms`
- `PUT /api/rooms/{id}`
- `DELETE /api/rooms/{id}`

### Reservations
- `GET /api/reservations`
- `GET /api/reservations/{id}`
- `GET /api/reservations?date=2026-05-10&status=confirmed&roomId=2`
- `POST /api/reservations`
- `PUT /api/reservations/{id}`
- `DELETE /api/reservations/{id}`

## Uruchomienie

```powershell
cd "C:\Users\Faded\Desktop\PJATK-APBD-Cw5-s33192\Program"
dotnet run
```

Dane startowe są przechowywane wyłącznie w pamięci aplikacji i resetują się po restarcie.

