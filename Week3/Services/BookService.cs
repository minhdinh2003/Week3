using System;
using System.Threading.Tasks;
using Week3.Models;
using Week3.Repository;
using Week3.Services.Interface;

namespace Week3.Services;

public class BookService(BookRepository book, ReviewRepository review) : IBookService
{
    private readonly BookRepository _bookRepository = book;
    private readonly ReviewRepository _reviewRepository = review;
    public async Task CreateBook(Book book)
    {
        await _bookRepository.AddAsync(book);
    }

    public async Task<Book> GetBookById(int id) => await _bookRepository.GetByIdAsync(id);

    public async Task<List<Book>> GetBooks()
    {
        return await _bookRepository.GetAllAsync();
    }

    public void RemoveBook(Book book)
    {
        _bookRepository.Delete(book);
        _reviewRepository.RemoveByBookIdAsync(book.Id);
    }

    public void UpdateBook(Book book)
    {
        _bookRepository.Update(book);
    }
}
