namespace QuizzHive.Server.DataLayer
{
    /// <summary>
    /// Represents opaque identifier assigned to specific version to perform optimistic concurrency check.
    /// Not yet saved entities have <see cref="NonExisting"/> value.
    /// </summary>
    public record struct VersionKey(string Key)
    {
        public static VersionKey NonExisting { get; } = new VersionKey("");
        public static VersionKey NewUnique() => new VersionKey(Guid.NewGuid().ToString("N"));
    }
}
