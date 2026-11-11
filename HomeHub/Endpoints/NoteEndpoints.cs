using System.Security.Claims;
using HomeHub.Hubs;
using HomeHub.Models;
using HomeHub.Models.Dtos;
using HomeHub.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace HomeHub.Endpoints;

public static class NoteEndpoints
{
    public static void MapNoteEndpoints(this WebApplication app)
    {
        app.MapGet("/api/notes", (string? kind, INoteRepository repository, ClaimsPrincipal user) =>
        {
            var notes = repository.GetAll(kind, user.GetUserId());
            return Results.Ok(notes);
        }).RequireAuthorization();

        app.MapGet("/api/notes/{id}", (int id, INoteRepository repository, ClaimsPrincipal user) =>
        {
            var note = repository.GetById(id, user.GetUserId());
            return note is null ? Results.NotFound() : Results.Ok(note);
        }).RequireAuthorization();

        app.MapPost("/api/notes", async (NoteRequest request, INoteRepository repository, IShoppingCategoryRepository categoryRepository, IHubContext<NotesHub> hub, ClaimsPrincipal user) =>
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return Results.BadRequest();

            var userId = user.GetUserId();

            if (request.CategoryId is int categoryId && categoryRepository.GetById(categoryId, userId) is null)
                return Results.BadRequest(new { error = "La etiqueta indicada no existe." });

            var note = new Note
            {
                UserId = userId,
                Content = request.Content,
                Kind = string.IsNullOrWhiteSpace(request.Kind) ? "note" : request.Kind,
                RoomId = request.RoomId,
                CategoryId = request.CategoryId
            };

            note.Id = repository.Save(note);
            await hub.Clients.Group(NotesHub.GroupName(userId)).SendAsync("notesChanged");
            var created = repository.GetById(note.Id, note.UserId);
            return Results.Created($"/api/notes/{note.Id}", created);
        }).RequireAuthorization();

        app.MapPut("/api/notes/{id}", async (int id, NoteRequest request, INoteRepository repository, IShoppingCategoryRepository categoryRepository, IHubContext<NotesHub> hub, ClaimsPrincipal user) =>
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return Results.BadRequest();

            var userId = user.GetUserId();
            var note = repository.GetById(id, userId);
            if (note is null)
                return Results.NotFound();

            if (request.CategoryId is int categoryId && categoryRepository.GetById(categoryId, userId) is null)
                return Results.BadRequest(new { error = "La etiqueta indicada no existe." });

            note.Content = request.Content;
            note.Kind = string.IsNullOrWhiteSpace(request.Kind) ? "note" : request.Kind;
            note.RoomId = request.RoomId;
            note.CategoryId = request.CategoryId;

            repository.Update(note);
            await hub.Clients.Group(NotesHub.GroupName(userId)).SendAsync("notesChanged");
            return Results.Ok(note);
        }).RequireAuthorization();

        app.MapPatch("/api/notes/{id}/toggle", async (int id, INoteRepository repository, IHubContext<NotesHub> hub, ClaimsPrincipal user) =>
        {
            var userId = user.GetUserId();
            var note = repository.GetById(id, userId);
            if (note is null)
                return Results.NotFound();

            note.IsDone = !note.IsDone;
            repository.Update(note);
            await hub.Clients.Group(NotesHub.GroupName(userId)).SendAsync("notesChanged");
            return Results.Ok(note);
        }).RequireAuthorization();

        app.MapDelete("/api/notes/{id}", async (int id, INoteRepository repository, IHubContext<NotesHub> hub, ClaimsPrincipal user) =>
        {
            var userId = user.GetUserId();
            var note = repository.GetById(id, userId);
            if (note is null)
                return Results.NotFound();

            repository.Delete(id, userId);
            await hub.Clients.Group(NotesHub.GroupName(userId)).SendAsync("notesChanged");
            return Results.NoContent();
        }).RequireAuthorization();
    }
}
