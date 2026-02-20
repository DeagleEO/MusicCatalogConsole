namespace MusicCatalogConsole.Models
{
    public class Release
    {
        public int Id { get; set; }
        public int ArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public string MediaType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; 
    }
}