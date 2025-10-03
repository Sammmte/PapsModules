namespace Paps.Physics
{
    public static class PhysicsSensorExtensions
    {
        public static bool FindsNothing(this PhysicsSensor sensor)
        {
            using var results = sensor.Sense();
            
            return results.Length == 0;
        }
    }
}