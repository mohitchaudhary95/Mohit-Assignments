namespace BookStoreAPI.Models
{
    /// <summary>
    /// Represents a Book entity in the store.
    /// </summary>
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public DateTime PublishedDate { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
