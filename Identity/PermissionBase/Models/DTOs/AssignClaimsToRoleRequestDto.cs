namespace PermissionBase.Models.DTOs
{
    public class AssignClaimsToRoleRequestDto
    {
        public Guid RoleId { get; set; }
        public List<string> ClaimNames { get; set; }

    }
}
