namespace Week3.Dto
{
    public class BookDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int AuthorId { get; set; }
    }
}
