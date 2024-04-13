using UnityEngine;

namespace Kickstarter.Inputs
{
    /// <summary>
    /// Represents an input asset for handling int-based input actions in Unity.
    /// Inherits the functionality from the InputAsset class for int input handling.
    /// </summary>
    [CreateAssetMenu(fileName = "Int Input", menuName = "Kickstarter/Inputs/Input Assets/Int")]
    public sealed class IntInput : InputAsset<int> { }
}
