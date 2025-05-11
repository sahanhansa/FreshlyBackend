using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class ItemCategory
    {
        public ItemCategory()
        {
            CategoryId = Guid.NewGuid();
        }

        // Primary Key
        [Key]
        public Guid CategoryId { get; set; }

        // Category Details
        public string CategoryName { get; set; }

        //Followings are the finalized item categories:
            //-Ladies
            //-Gents
            //-Kids
            //-Other

    }
}
