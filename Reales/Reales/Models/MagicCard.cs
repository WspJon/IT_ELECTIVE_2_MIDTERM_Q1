namespace MagicCardApp.Models
{
    public class MagicCard
    {
        public string Name { get; set; }
        public string TypeLine { get; set; }
        public string ImageUrl { get; set; }
        public string OracleText { get; set; }
        public string FlavorText { get; set; }
        public string PowerToughness { get; set; }
        public string Set { get; set; }
        public string Rarity { get; set; }
    }
}