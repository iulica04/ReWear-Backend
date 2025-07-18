﻿using Domain.Entities;

namespace Application.DTOs
{
   public class ClothingItemDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public required string Category { get; set; }
        public List<string> Tags { get; set; } = new();
        public required string Color { get; set; }
        public required string Brand { get; set; }
        public required string Material { get; set; }
        public string? PrintType { get; set; }
        public string? PrintDescription { get; set; }
        public string? Description { get; set; }

        public required string FrontImageUrl { get; set; }
        public required string BackImageUrl { get; set; }
        public uint? NumberOfWears { get; set; } = 0;
        public DateTime? LastWornDate { get; set; }
    }
}
