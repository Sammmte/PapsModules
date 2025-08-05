namespace Paps.Cheats
{
    public readonly struct CheatSubmenuAvailabilityInfo
    {
        public bool IsAvailable { get; }
        public string NotAvailableReason { get; }

        private CheatSubmenuAvailabilityInfo(bool isAvailable, string notAvailableReason)
        {
            IsAvailable = isAvailable;
            NotAvailableReason = notAvailableReason;
        }

        public static CheatSubmenuAvailabilityInfo Available() => new CheatSubmenuAvailabilityInfo(true, null);

        public static CheatSubmenuAvailabilityInfo NotAvailable(string reason) =>
            new CheatSubmenuAvailabilityInfo(false, reason);
    }
}