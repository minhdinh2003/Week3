using System.ComponentModel.DataAnnotations;

namespace Week3.Dto
{
    public class ReviewDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Rate { get; set; }
        public int ReviewerId { get; set; }
        public int BookId { get; set; }
    }
}
