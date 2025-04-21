using System.ComponentModel.DataAnnotations;
namespace FreshlyBackendNew.Models
{
    public class UserGroupPrivilege
    {
        [Key]
        public Guid UserGroupPrivilegeId { get; set; } = Guid.NewGuid();

        // Foreign Key for UserGroup
        public Guid UserGroupId { get; set; }
        public UserGroup UserGroup { get; set; }

        // Foreign Key for Privilege
        public Guid PrivilegeId { get; set; }
        public Privilege Privilege { get; set; }
    }
}
