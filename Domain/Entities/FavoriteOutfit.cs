namespace Domain.Entities
{
    public class FavoriteOutfit
    {
        public  Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required Guid OutfitId { get; set; }
    }
}
