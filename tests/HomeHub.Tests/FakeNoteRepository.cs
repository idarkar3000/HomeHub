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

    public List<Note> GetAll(string? kind)
    {
        if (string.IsNullOrEmpty(kind))
        {
            return _notes;
        }

        return _notes.Where(note => note.Kind == kind).ToList();
    }

    public Note? GetById(int id)
    {
        return _notes.FirstOrDefault(note => note.Id == id);
    }

    public void Update(Note note)
    {
        var index = _notes.FindIndex(n => n.Id == note.Id);
        if (index >= 0)
        {
            _notes[index] = note;
        }
    }

    public void Delete(int id)
    {
        _notes.RemoveAll(note => note.Id == id);
    }
}
