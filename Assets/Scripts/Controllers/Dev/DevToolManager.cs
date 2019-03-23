using UnityEngine;

namespace Controllers.Dev
{
    public class DevToolManager : MonoBehaviour
    {
        public static DevToolManager Instance { get; private set; }

        private TileNodesTool tileNodesTool;
        
        private void Awake()
        {
            Instance = this;

            tileNodesTool = GetComponent<TileNodesTool>();
        }
        
        public void ToggleTileNodes()
        {
            tileNodesTool.enabled = !tileNodesTool.enabled;
        }
    }
}