namespace SpacePirates.API.Models.DTOs
{
    public class ResourceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public double WeightPerUnit { get; set; }
        public string Description { get; set; } = string.Empty;
    }
} 