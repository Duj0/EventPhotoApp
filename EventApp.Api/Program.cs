using Dapper;
using EventApp.Api.DTOs;
using Microsoft.AspNetCore.Mvc.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.MapPost("/photos", async(SavePhotoDTO photoDTO) =>
{
    if (string.IsNullOrEmpty(photoDTO.photoUrl))
    {
        return Results.BadRequest("Photo url is required.");
    }

    if (photoDTO.eventId == Guid.Empty)
    {
        return Results.BadRequest("Event id is required.");
    }

    await using var connection = new Npgsql.NpgsqlConnection(connectionString);

    var sql = """
    INSERT INTO photos (event_id, photo_url, uploaded_by)
    VALUES (@eventId, @photoUrl, @uploadedBy)
    returning id;
    """;

    var photoId = await connection.ExecuteScalarAsync<Guid>(sql, new { photoDTO.eventId,photoDTO.photoUrl,photoDTO.uploadedBy });
    return Results.Created($"/photos/{photoId}", new
    {
        Id = photoId,
        photoDTO.eventId,
        photoDTO.photoUrl,
        photoDTO.uploadedBy
    });


});

app.MapPost("/events",  async (CreateEventDto eventDto) =>
{
    var code = Random.Shared.Next(100000, 999999).ToString();

    if (string.IsNullOrEmpty(eventDto.Name))
    {
        return Results.BadRequest("Event name is required.");
    }

    if (string.IsNullOrEmpty(eventDto.TimeOfEvent))
    {
        return Results.BadRequest("Time of event is required.");
    }

    if (string.IsNullOrEmpty(eventDto.DateOfEvent))
    {
        return Results.BadRequest("Date of event is required.");
    }

    await using var connection = new Npgsql.NpgsqlConnection(connectionString);

    var sql = """
    INSERT INTO events (name, "timeOfEvent", "dateOfEvent", code)
    VALUES (@Name, @TimeOfEvent, @DateOfEvent, @Code)
    returning id;
    """;

    var eventId = await connection.ExecuteScalarAsync<Guid>(sql, new { eventDto.Name, eventDto.TimeOfEvent, eventDto.DateOfEvent, Code = code });
    return Results.Created($"/events/{eventId}", new
    {
        Id = eventId,
        eventDto.Name,
        eventDto.TimeOfEvent,
        eventDto.DateOfEvent,
        Code  = code
    });

});

app.MapGet("/events/{code}", async (string code) =>
{
    await using var connection = new Npgsql.NpgsqlConnection(connectionString);
    var sql = """
    SELECT id, name, "timeOfEvent", "dateOfEvent", code
    FROM events
    WHERE code = @Code;
    """;
    var eventData = await connection.QueryFirstOrDefaultAsync(sql, new { Code = code });
    if (eventData == null)
    {
        return Results.NotFound("Event not found.");
    }
    return Results.Ok(eventData);
});

app.MapGet("/photos/{eventId}", async(Guid eventId)=>
{
    await using var connection = new Npgsql.NpgsqlConnection(connectionString);
    var sql = """
    SELECT photo_url, uploaded_by
    FROM photos
    WHERE event_id = @event_id;
    """;
    var eventData = await connection.QueryAsync(sql, new { event_id = eventId });
    if (!eventData.Any())
    {
        return Results.NotFound("Event not found.");
    }
    return Results.Ok(eventData);

});

app.MapGet("/events/byId/{eventid}", async(Guid eventid)=>
{
    await using var connection = new Npgsql.NpgsqlConnection(connectionString);
    var sql = """
    SELECT id, name, "timeOfEvent", "dateOfEvent", code
    FROM events
    WHERE id = @eventid;
    """;

    var eventData = await connection.QueryFirstOrDefaultAsync(sql, new { eventid = eventid });
    if (eventData == null)
    {
        return Results.NotFound("Event not found.");
    }
    return Results.Ok(eventData);
});


app.MapGet("/health", () => Results.Ok("OK"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
