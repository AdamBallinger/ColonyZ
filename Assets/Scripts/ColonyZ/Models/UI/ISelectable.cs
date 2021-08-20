using UnityEngine;

namespace ColonyZ.Models.UI
{
    public interface ISelectable
    {
        /// <summary>
        ///     The name that appears at the top of the selection UI.
        /// </summary>
        /// <returns></returns>
        string GetSelectionName();

        /// <summary>
        ///     The contents of the description field for the selection UI.
        /// </summary>
        /// <returns></returns>
        string GetSelectionDescription();

        /// <summary>
        ///     Returns the position of the selectable to display in the UI.
        /// </summary>
        /// <returns></returns>
        Vector2 GetPosition();
    }
}