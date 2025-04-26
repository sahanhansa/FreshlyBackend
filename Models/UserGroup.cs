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

    }
}

