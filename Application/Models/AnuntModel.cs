namespace Application.Models
{
    public class AnuntModel
    {
        public string Titlu { get; set; } // Titlul anunțului
        public string Descriere { get; set; } // Descrierea anunțului
        public decimal Pret { get; set; } // Prețul anunțului
        public string CaleImagine { get; set; } // Calea către imaginea anunțului (ex: "C:\\imagine.jpg")
    }
}
