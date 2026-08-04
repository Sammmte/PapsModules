using System;

namespace Paps.Persistence
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DataStorageValueTypeAttribute : Attribute
    {
        public string TypeDiscriminator { get; }
        public int InitialPoolCapacity { get; }

        public DataStorageValueTypeAttribute(string typeDiscriminator, int initialPoolCapacity = 10)
        {
            TypeDiscriminator = typeDiscriminator;
            InitialPoolCapacity = initialPoolCapacity;
        }
    }
}