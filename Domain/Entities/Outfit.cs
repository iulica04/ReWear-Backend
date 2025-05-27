namespace Domain.Entities
{
    public class Outfit
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Name { get; set; }
    public string? Style { get; set; }
    public List<OutfitClothingItem> OutfitClothingItems { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string? Season { get; set; }
    public string? Description { get; set; }
    public required string ImageUrl { get; set; }
    public float[]? Embedding { get; set; }
}
}
