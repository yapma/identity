namespace SimpleClaimBase.Models.DTOs
{
    public class AddClaimToUserDto
    {
        public string UserId { get; set; }
        public List<string> ClaimNames { get; set; }
    }
}
