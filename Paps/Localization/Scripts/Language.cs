namespace Paps.Localization
{
    public readonly struct Language
    {
        public string Id { get; }
        public string Name { get; }

        public Language(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
