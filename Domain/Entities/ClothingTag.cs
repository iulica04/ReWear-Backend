using Domain.Entities;

namespace Domain.Models
{
    public class ClothingTag
    {
        public Guid Id { get; set; }
        public Guid ClothingItemId { get; set; }
        public string Tag { get; set; } = string.Empty;

        public ClothingItem ClothingItem { get; set; } = null!;
    }
}
