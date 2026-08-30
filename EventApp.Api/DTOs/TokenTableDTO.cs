namespace EventApp.Api.DTOs
{
    public record TokenTableDTO(string token, Guid eventId, string role);
}
