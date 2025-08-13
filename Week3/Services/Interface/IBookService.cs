using System;
using Week3.Models;

namespace Week3.Services.Interface;

public interface IBookService
{
    Task<Book> GetBookById(int id);
    Task<List<Book>> GetBooks();
    Task CreateBook(Book book);
    void RemoveBook(Book book);
    void UpdateBook(Book book);
}
