using UnityEngine;

namespace Controllers.UI.Toolbar
{
    public class TBRootButton : MonoBehaviour
    {
        [SerializeField]
        private TBMenuController menuObject;
        
        public void OnButtonClick()
        {
            FindObjectOfType<TBController>().OpenMenu(menuObject);
        }
    }
}
