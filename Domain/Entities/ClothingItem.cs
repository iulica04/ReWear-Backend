using Domain.Models;

namespace Domain.Entities
{
    public class ClothingItem
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public required string Category { get; set; }
        public List<ClothingTag> Tags { get; set; } = new();
        public required string Color { get; set; }
        public required string Brand { get; set; }
        public required string Material { get; set; }
        public string? PrintType { get; set; }
        public string? PrintDescription { get; set; }
        public string? Description { get; set; }
        public required string FrontImageUrl { get; set; }
        public string? BackImageUrl { get; set; }
        public float[]? Embedding { get; set; }
        public DateTime CreatedAt { get; set; }
        public uint? NumberOfWears { get; set; } = 0;

        public List<OutfitClothingItem> OutfitClothingItems { get; set; } = new();
    }
}
