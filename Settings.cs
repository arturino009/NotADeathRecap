using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace NotADeathRecap
{
    public class Settings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(true);
        [Menu("Saving buffs for the last # seconds before death")]
        public RangeNode<int> BuffTTL { get; set; } = new RangeNode<int>(1, 0, 10);
    }
}