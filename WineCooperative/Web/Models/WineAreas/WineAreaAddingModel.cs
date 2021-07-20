using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models.WineAreas
{
    public class WineAreaAddingModel
    {
        [Required]
        [StringLength(WineAreaMaxLength, MinimumLength =WineAreaMinLength)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
