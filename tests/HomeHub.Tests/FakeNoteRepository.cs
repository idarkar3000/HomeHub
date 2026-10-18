using HomeHub.Models;
using HomeHub.Repositories;

namespace HomeHub.Tests;

public class FakeNoteRepository : INoteRepository
{
    private readonly List<Note> _notes = new();
    private int _lastId;

    public void InitializeDatabase()
    {
    }

    public int Save(Note note)
    {
        note.Id = ++_lastId;
        note.CreatedAt = DateTime.Now;
        _notes.Add(note);
        return note.Id;
    }

    public List<Note> GetAll(string? kind, int userId)
    {
        var query = _notes.Where(note => note.UserId == userId);

        if (!string.IsNullOrEmpty(kind))
        {
            query = query.Where(note => note.Kind == kind);
        }

        return query.ToList();
    }

    public Note? GetById(int id, int userId)
    {
        return _notes.FirstOrDefault(note => note.Id == id && note.UserId == userId);
    }

    public void Update(Note note)
    {
        var index = _notes.FindIndex(n => n.Id == note.Id);
        if (index >= 0)
        {
            _notes[index] = note;
        }
    }

    public void Delete(int id, int userId)
    {
        _notes.RemoveAll(note => note.Id == id && note.UserId == userId);
    }
}
