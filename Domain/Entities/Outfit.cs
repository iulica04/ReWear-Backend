namespace Domain.Entities
{
    public class Outfit
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public List<ClothingItem> ClothingItems { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string? Season { get; set; }
        public string? Description { get; set; }
        public required string ImageUrl { get; set; }
    }
}
