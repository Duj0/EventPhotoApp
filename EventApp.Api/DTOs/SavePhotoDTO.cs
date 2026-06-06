namespace EventApp.Api.DTOs
{
    public record SavePhotoDTO(Guid eventId, string photoUrl, string uploadedBy);

}
