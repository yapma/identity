namespace MixClaimRole.Models.DTOs
{
    public class AssignRoleToUserRequestDto
    {
        public Guid UserId { get; set; }
        public List<Guid> RolesId { get; set; }
    }
}
