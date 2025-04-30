namespace Paps.Build
{
    public interface IBuildPreprocessor
    {
        public int Order { get => 0; }
        public void Process(BuildSettings currentBuildSettings);
    }
}