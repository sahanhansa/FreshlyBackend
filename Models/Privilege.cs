using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Privilege
    {
        public Privilege()
        {
            PrivilegeId = Guid.NewGuid();
        }

        [Key]
        public Guid PrivilegeId { get; set; }

        [Required]
        public required string PrivilegeName { get; set; }

        // Navigation property for many-to-many relationship
        public ICollection<PrivilegeUserGroup> PrivilegeUserGroups { get; set; }
    }
}
