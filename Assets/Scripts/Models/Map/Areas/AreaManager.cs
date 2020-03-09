namespace Models.Map.Areas
{
    public class AreaManager
    {
        public static AreaManager Instance { get; private set; }

        private AreaManager()
        {
            
        }
        
        public static void Create()
        {
            if (Instance == null)
            {
                Instance = new AreaManager();
            }
        }
    }
}