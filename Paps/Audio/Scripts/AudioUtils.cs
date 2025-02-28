using UnityEngine;

namespace Paps.Audio
{
    public static class AudioUtils
    {
        public static float ConvertToDb(float value) => Mathf.Log10(value) * 20;
    }
}
