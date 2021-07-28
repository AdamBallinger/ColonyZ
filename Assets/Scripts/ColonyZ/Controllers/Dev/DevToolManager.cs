using UnityEngine;

namespace ColonyZ.Controllers.Dev
{
    public class DevToolManager : MonoBehaviour
    {
        private AreasDebugTool areasDebugTool;
        private ItemsDevTool itemDevTool;

        [SerializeField] private GameObject jobsInfoRoot;
        private PathDebugTool pathDebugTool;
        private RegionsDebugTool regionsDebugTool;

        private TileNodesTool tileNodesTool;
        public static DevToolManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            tileNodesTool = FindObjectOfType<TileNodesTool>();
            areasDebugTool = FindObjectOfType<AreasDebugTool>();
            itemDevTool = FindObjectOfType<ItemsDevTool>();
            regionsDebugTool = FindObjectOfType<RegionsDebugTool>();
            pathDebugTool = FindObjectOfType<PathDebugTool>();
        }

        public void ToggleTileNodes()
        {
            tileNodesTool.Toggle();
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
            pathDebugTool.Toggle();
        }

        public void ToggleJobsInfo()
        {
            jobsInfoRoot.SetActive(!jobsInfoRoot.activeSelf);
        }
    }
}