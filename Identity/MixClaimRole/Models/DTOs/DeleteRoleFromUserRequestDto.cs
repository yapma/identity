namespace MixClaimRole.Models.DTOs
{
    public class DeleteRoleFromUserRequestDto
    {
        public Guid UserId { get; set; }
        public List<Guid> RolesId { get; set; }
    }
}
