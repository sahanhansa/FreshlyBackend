using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class PrivilegeUserGroup
    {
        // Foreign Keys
        public Guid? PrivilegeId { get; set; }
        public Guid? UserGroupId { get; set; }

        // Navigation Properties
        public Privilege? Privilege { get; set; }
        public UserGroup? UserGroup { get; set; }
    }
}

