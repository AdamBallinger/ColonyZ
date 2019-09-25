using UnityEngine;

namespace Models.UI
{
    public interface ISelectable
    {
        /// <summary>
        /// The icon to display on the selection UI. E.g Item icon, character face etc.
        /// </summary>
        /// <returns></returns>
        Sprite GetSelectionIcon();

        /// <summary>
        /// The name that appears at the top of the selection UI.
        /// </summary>
        /// <returns></returns>
        string GetSelectionName();

        /// <summary>
        /// The contents of the description field for the selection UI.
        /// </summary>
        /// <returns></returns>
        string GetSelectionDescription();
    }
}