namespace Project.DTO
{
    public class CreateUserRequest
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public string RoleName { get; set; }

        public int? UniversityId { get; set; }

        public int? DepartmentId { get; set; }

        public int? InstitutionId { get; set; }
    }
}
