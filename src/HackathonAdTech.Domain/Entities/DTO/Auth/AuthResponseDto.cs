namespace HackathonAdTech.Domain.Entities.DTO.Auth
{
    public class AuthResponseDto
    {
        public bool IsAuthSuccessful { get; set; }        
        public string? ErrorMessage { get; set; }
        public string? Token { get; set; }
    }
}
