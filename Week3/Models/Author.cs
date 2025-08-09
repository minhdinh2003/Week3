using System.ComponentModel.DataAnnotations;

namespace Week3.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }
        public ICollection<Book> Books { get; set; } = [];
    }
}
