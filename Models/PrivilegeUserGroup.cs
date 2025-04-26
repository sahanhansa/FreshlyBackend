using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class PrivilegeUserGroup
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Foreign key for Privilege
        public Guid PrivilegeId { get; set; }
        public Privilege Privilege { get; set; }

        // Foreign key for UserGroup
        public Guid UserGroupId { get; set; }
        public UserGroup UserGroup { get; set; }
    }
}

