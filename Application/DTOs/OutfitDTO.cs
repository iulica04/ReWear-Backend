using Domain.Entities;

namespace Application.DTOs
{
    public class OutfitDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public List<ClothingItemDTO> ClothingItemDTOs { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string? Season { get; set; }
        public string? Description { get; set; }
        public required string ImageUrl { get; set; }
    }
}
