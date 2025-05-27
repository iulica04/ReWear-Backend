namespace Domain.Entities
{
    public class OutfitClothingItem
    {
        public Guid OutfitId { get; set; }
        public Outfit Outfit { get; set; } = null!;

        public Guid ClothingItemId { get; set; }
        public ClothingItem ClothingItem { get; set; } = null!;
    }
}
