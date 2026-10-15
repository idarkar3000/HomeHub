using HomeHub.Models;
using HomeHub.Models.Dtos;
using HomeHub.Repositories;

namespace HomeHub.Endpoints
{
    public static class NoteEndpoints
    {
        public static void MapNoteEndpoints(this WebApplication app)
        {
            app.MapGet("/api/notes", (string? kind, INoteRepository repository) =>
            {
                var notes = repository.GetAll(kind);
                return Results.Ok(notes);
            });

            app.MapGet("/api/notes/{id}", (int id, INoteRepository repository) =>
            {
                var note = repository.GetById(id);
                return note is null ? Results.NotFound() : Results.Ok(note);
            });

            app.MapPost("/api/notes", (NoteRequest request, INoteRepository repository) =>
            {
                if (string.IsNullOrWhiteSpace(request.Content))
                    return Results.BadRequest();

                var note = new Note
                {
                    Content = request.Content,
                    Kind = string.IsNullOrWhiteSpace(request.Kind) ? "note" : request.Kind,
                    RoomId = request.RoomId
                };

                note.Id = repository.Save(note);
                var created = repository.GetById(note.Id);
                return Results.Created($"/api/notes/{note.Id}", created);
            });

            app.MapPut("/api/notes/{id}", (int id, NoteRequest request, INoteRepository repository) =>
            {
                if (string.IsNullOrWhiteSpace(request.Content))
                    return Results.BadRequest();

                var note = repository.GetById(id);
                if (note is null)
                    return Results.NotFound();

                note.Content = request.Content;
                note.Kind = string.IsNullOrWhiteSpace(request.Kind) ? "note" : request.Kind;
                note.RoomId = request.RoomId;

                repository.Update(note);
                return Results.Ok(note);
            });

            app.MapPatch("/api/notes/{id}/toggle", (int id, INoteRepository repository) =>
            {
                var note = repository.GetById(id);
                if (note is null)
                    return Results.NotFound();

                note.IsDone = !note.IsDone;
                repository.Update(note);
                return Results.Ok(note);
            });

            app.MapDelete("/api/notes/{id}", (int id, INoteRepository repository) =>
            {
                var note = repository.GetById(id);
                if (note is null)
                    return Results.NotFound();

                repository.Delete(id);
                return Results.NoContent();
            });
        }
    }
}
