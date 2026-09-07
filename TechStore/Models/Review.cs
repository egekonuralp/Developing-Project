namespace TechStore.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public AppUser User { get; set; } = null!;

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
    }
}
