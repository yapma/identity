namespace SimpleClaimBase.Models.DTOs
{
    public class DeleteClaimFromUserDto
    {
        public string UserId { get; set; }
        public List<string> ClaimNames { get; set; }
    }
}
