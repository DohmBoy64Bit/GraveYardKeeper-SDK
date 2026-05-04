using System.Collections.Generic;

namespace GraveSDK.Core
{
    /// <summary>
    /// Interface for custom unlock conditions
    /// </summary>
    public interface IUnlockCondition
    {
        bool IsUnlocked();
        string GetDescription();
    }

    /// <summary>
    /// Interface for custom craft cost calculators
    /// </summary>
    public interface ICraftCostCalculator
    {
        string GetDisplayText(WorldGameObject wgo, int multiplier);
        bool CanCraft(WorldGameObject wgo);
    }
}
