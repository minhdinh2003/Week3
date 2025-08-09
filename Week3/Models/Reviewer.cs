using System.ComponentModel.DataAnnotations;
using Week3.MongoModels;

namespace Week3.Models
{
    public class Reviewer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }
        public ICollection<Review> Reviews { get; set; } = [];
    }
}
