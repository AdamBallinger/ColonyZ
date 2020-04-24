using UnityEngine;

namespace Controllers.Dev
{
    public class DevToolManager : MonoBehaviour
    {
        public static DevToolManager Instance { get; private set; }

        [SerializeField] private GameObject pathDebugRoot;
        [SerializeField] private GameObject jobsInfoRoot;

        private TileNodesTool tileNodesTool;
        private ItemsDevTool itemDevTool;
        private AreasDebugTool areasDebugTool;
        private RegionsDebugTool regionsDebugTool;

        private void Awake()
        {
            Instance = this;

            tileNodesTool = GetComponent<TileNodesTool>();
            areasDebugTool = FindObjectOfType<AreasDebugTool>();
            itemDevTool = FindObjectOfType<ItemsDevTool>();
            regionsDebugTool = FindObjectOfType<RegionsDebugTool>();
        }

        public void ToggleTileNodes()
        {
            tileNodesTool.enabled = !tileNodesTool.enabled;
        }

        public void DisableTileNodes()
        {
            tileNodesTool.enabled = false;
        }

        public void ToggleAreasDebug()
        {
            areasDebugTool.Toggle();
        }

        public void ToggleRegionsDebug()
        {
            regionsDebugTool.Toggle();
        }

        public void ToggleItemTool()
        {
            itemDevTool.Toggle();
        }

        public void TogglePathDebug()
        {
            pathDebugRoot.SetActive(!pathDebugRoot.activeSelf);
        }

        public void ToggleJobsInfo()
        {
            jobsInfoRoot.SetActive(!jobsInfoRoot.activeSelf);
        }
    }
}