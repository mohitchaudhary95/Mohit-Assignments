using BookStoreAPI.Data;
using BookStoreAPI.DTOs;
using BookStoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookResponseDto>> GetAllBooksAsync(string? genre, bool? isAvailable);
        Task<BookResponseDto?> GetBookByIdAsync(int id);
        Task<BookResponseDto> CreateBookAsync(CreateBookDto dto);
        Task<BookResponseDto?> UpdateBookAsync(int id, UpdateBookDto dto);
        Task<bool> DeleteBookAsync(int id);
        Task<IEnumerable<BookResponseDto>> SearchBooksAsync(string keyword);
    }

    public class BookService : IBookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookResponseDto>> GetAllBooksAsync(string? genre, bool? isAvailable)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(genre))
                query = query.Where(b => b.Genre.ToLower() == genre.ToLower());

            if (isAvailable.HasValue)
                query = query.Where(b => b.IsAvailable == isAvailable.Value);

            var books = await query.ToListAsync();
            return books.Select(MapToDto);
        }

        public async Task<BookResponseDto?> GetBookByIdAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            return book == null ? null : MapToDto(book);
        }

        public async Task<BookResponseDto> CreateBookAsync(CreateBookDto dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Genre = dto.Genre,
                Price = dto.Price,
                Stock = dto.Stock,
                PublishedDate = dto.PublishedDate,
                IsAvailable = dto.Stock > 0
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return MapToDto(book);
        }

        public async Task<BookResponseDto?> UpdateBookAsync(int id, UpdateBookDto dto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            if (dto.Title != null) book.Title = dto.Title;
            if (dto.Author != null) book.Author = dto.Author;
            if (dto.Genre != null) book.Genre = dto.Genre;
            if (dto.Price.HasValue) book.Price = dto.Price.Value;
            if (dto.Stock.HasValue)
            {
                book.Stock = dto.Stock.Value;
                book.IsAvailable = book.Stock > 0;
            }
            if (dto.IsAvailable.HasValue) book.IsAvailable = dto.IsAvailable.Value;

            await _context.SaveChangesAsync();
            return MapToDto(book);
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BookResponseDto>> SearchBooksAsync(string keyword)
        {
            var lower = keyword.ToLower();
            var books = await _context.Books
                .Where(b => b.Title.ToLower().Contains(lower) ||
                            b.Author.ToLower().Contains(lower) ||
                            b.Genre.ToLower().Contains(lower))
                .ToListAsync();

            return books.Select(MapToDto);
        }

        private static BookResponseDto MapToDto(Book b) => new()
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            Genre = b.Genre,
            Price = b.Price,
            Stock = b.Stock,
            PublishedDate = b.PublishedDate,
            IsAvailable = b.IsAvailable
        };
    }
}
