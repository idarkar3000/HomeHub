using HomeHub.Models;
using HomeHub.Repositories;

namespace HomeHub.Tests;

public class FakeRoomRepository : IRoomRepository
{
    private readonly List<Room> _rooms = new();
    private int _lastId;

    public void InitializeDatabase()
    {
    }

    public int Save(Room room)
    {
        room.Id = ++_lastId;
        _rooms.Add(room);
        return room.Id;
    }

    public List<Room> GetAll()
    {
        return _rooms;
    }

    public Room? GetById(int id)
    {
        return _rooms.FirstOrDefault(room => room.Id == id);
    }

    public void Update(Room room)
    {
        var index = _rooms.FindIndex(r => r.Id == room.Id);
        if (index >= 0)
        {
            _rooms[index] = room;
        }
    }

    public void Delete(int id)
    {
        _rooms.RemoveAll(room => room.Id == id);
    }
}
