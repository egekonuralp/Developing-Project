namespace TechStore.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public AppUser User { get; set; } = null!;

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}
