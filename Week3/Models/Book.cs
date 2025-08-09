using System.ComponentModel.DataAnnotations;
using Week3.MongoModels;

namespace Week3.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Title { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public double Rate { get; set; }
        public ICollection<Review> Reviews { get; set; } = [];
    }
}
