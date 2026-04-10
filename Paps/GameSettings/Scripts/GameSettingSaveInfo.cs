using Unity.Properties;

namespace Paps.GameSettings
{
    [GeneratePropertyBag]
    public abstract class GameSettingSaveInfo
    {
        
    }

    [GeneratePropertyBag]
    public class GameSettingSaveInfo<T> : GameSettingSaveInfo
    {
        [CreateProperty] public T Value { get; private set; }

        public GameSettingSaveInfo() { }

        public GameSettingSaveInfo(T value)
        {
            Value = value;
        }
    }
}
