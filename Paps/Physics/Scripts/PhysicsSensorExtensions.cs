namespace Paps.Physics
{
    public static class PhysicsSensorExtensions
    {
        public static bool FindsNothing(this PhysicsSensor sensor)
        {
            return sensor.Sense().Length == 0;
        }
    }
}