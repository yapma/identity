namespace SimpleRoleBase.Models.DTOs
{
    public class DeleteRolesFromUserRequestDto
    {
        public Guid UserId { get; set; }
        public List<Guid> RolesId { get; set; }
    }
}
