namespace Paps.SceneLoading
{
    public struct SceneGroup
    {
        public Scene[] Scenes { get; }

        public SceneGroup(params Scene[] scenes)
        {
            Scenes = scenes;
        }
    }
}
