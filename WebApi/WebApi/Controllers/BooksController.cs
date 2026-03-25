using BookStoreAPI.DTOs;
using BookStoreAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.Controllers
{
    /// <summary>
    /// Manages Book resources in the Book Store.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // ─────────────────────────────────────────────
        // GET /api/books
        // ─────────────────────────────────────────────
        /// <summary>
        /// Retrieves all books with optional filters.
        /// </summary>
        /// <param name="genre">Filter by genre (e.g. Fiction, Programming)</param>
        /// <param name="isAvailable">Filter by availability</param>
        /// <returns>List of books</returns>
        /// <response code="200">Returns the list of books</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? genre,
            [FromQuery] bool? isAvailable)
        {
            var books = await _bookService.GetAllBooksAsync(genre, isAvailable);
            return Ok(books);
        }

        // ─────────────────────────────────────────────
        // GET /api/books/{id}
        // ─────────────────────────────────────────────
        /// <summary>
        /// Retrieves a single book by ID.
        /// </summary>
        /// <param name="id">The book ID</param>
        /// <returns>A book object</returns>
        /// <response code="200">Book found and returned</response>
        /// <response code="404">Book not found</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(BookResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound(new { message = $"Book with ID {id} was not found." });

            return Ok(book);
        }

        // ─────────────────────────────────────────────
        // GET /api/books/search?keyword=xyz
        // ─────────────────────────────────────────────
        /// <summary>
        /// Search books by title, author, or genre keyword.
        /// </summary>
        /// <param name="keyword">Search keyword</param>
        /// <returns>Matching books</returns>
        /// <response code="200">Returns matching books</response>
        /// <response code="400">Keyword is required</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<BookResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Search keyword is required." });

            var results = await _bookService.SearchBooksAsync(keyword);
            return Ok(results);
        }

        // ─────────────────────────────────────────────
        // POST /api/books
        // ─────────────────────────────────────────────
        /// <summary>
        /// Creates a new book.
        /// </summary>
        /// <param name="dto">Book creation payload</param>
        /// <returns>Newly created book</returns>
        /// <response code="201">Book created successfully</response>
        /// <response code="400">Validation error</response>
        [HttpPost]
        [ProducesResponseType(typeof(BookResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateBookDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _bookService.CreateBookAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // ─────────────────────────────────────────────
        // PUT /api/books/{id}
        // ─────────────────────────────────────────────
        /// <summary>
        /// Updates an existing book (partial or full update).
        /// </summary>
        /// <param name="id">The book ID to update</param>
        /// <param name="dto">Fields to update</param>
        /// <returns>Updated book</returns>
        /// <response code="200">Book updated successfully</response>
        /// <response code="404">Book not found</response>
        /// <response code="400">Validation error</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(BookResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _bookService.UpdateBookAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = $"Book with ID {id} was not found." });

            return Ok(updated);
        }

        // ─────────────────────────────────────────────
        // DELETE /api/books/{id}
        // ─────────────────────────────────────────────
        /// <summary>
        /// Deletes a book by ID.
        /// </summary>
        /// <param name="id">The book ID to delete</param>
        /// <response code="204">Book deleted successfully</response>
        /// <response code="404">Book not found</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _bookService.DeleteBookAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Book with ID {id} was not found." });

            return NoContent();
        }
    }
}
