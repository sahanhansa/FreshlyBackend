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

        public required string PrivilegeName { get; set; }
    }
}
