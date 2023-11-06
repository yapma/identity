namespace SimpleRoleBase.Models.DTOs
{
    public class AssignRolesToUserRequestDto
    {
        public Guid UserId { get; set; }
        public List<Guid> RolesId { get; set; }
    }
}
