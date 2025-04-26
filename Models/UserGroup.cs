using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class UserGroup
    {
        public UserGroup()
        {
            UserGroupId = Guid.NewGuid();
        }

        [Key]
        public Guid UserGroupId { get; set; }

        [Required]
        public required string UserGroupName { get; set; }

        // Navigation property for many-to-many relationship
        public ICollection<PrivilegeUserGroup> PrivilegeUserGroups { get; set; }

        // Navigation property for one-to-many relationship
        public ICollection<User> Users { get; set; }

    }
}

