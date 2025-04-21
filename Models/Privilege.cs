using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Privilege
    {
        public Privilege()
        {
            PrivilegeId = Guid.NewGuid();
            UserGroupPrivileges = new List<UserGroupPrivilege>();
        }

        [Key]
        public Guid PrivilegeId { get; set; }

        [Required]
        public required string PrivilegeName { get; set; }

        // Many-to-Many Relationship with UserGroup
        public ICollection<UserGroupPrivilege> UserGroupPrivileges { get; set; }
    }
}
