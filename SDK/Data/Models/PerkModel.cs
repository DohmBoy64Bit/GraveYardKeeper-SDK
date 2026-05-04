namespace GraveSDK.Data.Models
{
    public class PerkModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string IconName { get; set; }
        public float Stars { get; set; }
        public bool IsUnlocked { get; set; }
        public bool ShowInUI { get; set; }
    }
}
