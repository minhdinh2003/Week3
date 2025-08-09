using AutoMapper;
using Week3.Models;
using Week3.Dto;
using Week3.MongoModels;

namespace Week3.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Author
            CreateMap<Author, AuthorDto>();
            CreateMap<AuthorDto, Author>();

            // Book
            CreateMap<Book, BookDto>();
            CreateMap<BookDto, Book>();
            // Reviewer
            CreateMap<Reviewer, ReviewerDto>();
            CreateMap<ReviewerDto, Reviewer>();

            // Review
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.Book.Id))
                .ForMember(dest => dest.ReviewerId, opt => opt.MapFrom(src => src.Reviewer.Id))
                .ReverseMap();
        }
    }
}
