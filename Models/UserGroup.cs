using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class UserGroup
    {
        public UserGroup()
        {
            UserGroupId = Guid.NewGuid(); 
            PrivilegeUserGroups = new List<PrivilegeUserGroup>();
            Users = new List<User>();
        }

        // Primary Key
        [Key]
        public Guid UserGroupId { get; set; }

        // User Group Details
        public string? UserGroupName { get; set; }

        // Navigation Properties
        public ICollection<PrivilegeUserGroup>? PrivilegeUserGroups { get; set; }
        public ICollection<User>? Users { get; set; }
    }
}

