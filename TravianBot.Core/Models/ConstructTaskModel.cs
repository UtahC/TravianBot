using TravianBot.Core.Enums;

namespace TravianBot.Core.Models
{
    public class ConstructTaskModel
    {
        public Buildings BuildingType { get; set; }
        public int BuildingId { get; set; }
        public bool IsConstrution { get; set; }
        public int LevelAfterWork { get; set; }
    }
}