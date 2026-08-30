namespace EventApp.Api.DTOs
{
    public record SendNotificationDTO(Guid eventId, string message);
}
