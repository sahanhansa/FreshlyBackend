using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class UserGroup
    {
        public UserGroup()
        {
            UserGroupId = Guid.NewGuid();
            Users = new List<User>();
            UserGroupPrivileges = new List<UserGroupPrivilege>();
        }

        [Key]
        public Guid UserGroupId { get; set; }

        [Required]
        public required string UserGroupName { get; set; }

        // One-to-Many Relationship with User
        public ICollection<User> Users { get; set; }

        // Many-to-Many Relationship with Privilege
        public ICollection<UserGroupPrivilege> UserGroupPrivileges { get; set; }
    }
}
