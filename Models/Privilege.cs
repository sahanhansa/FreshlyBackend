using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Privilege
    {
        public Privilege()
        {
            PrivilegeId = Guid.NewGuid();
            PrivilegeUserGroups = new List<PrivilegeUserGroup>();
        }

        // Primary Key
        [Key]
        public Guid PrivilegeId { get; set; }

        // Privilege Details
        public string? PrivilegeName { get; set; }

        // Navigation Properties
        public ICollection<PrivilegeUserGroup>? PrivilegeUserGroups { get; set; }
    }
}
