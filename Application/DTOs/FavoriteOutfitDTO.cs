namespace Application.DTOs
{
    public class FavoriteOutfitDTO
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required Guid OutfitId { get; set; }
    }
}
