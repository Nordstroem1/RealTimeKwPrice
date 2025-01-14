namespace Application.DTO.Role.SuperAdmin
{
    public class ChangeRoleAsSuperAdminDto
    {
        public Guid loggedInUserId { get; set; }
        public bool IsSuperAdmin { get; set; }
        public Guid UserId { get; set; }
        public string NewRole { get; set; }
    }
}
