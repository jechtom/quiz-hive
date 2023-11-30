namespace QuizHive.Server.DataLayer
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Gets instance of <see cref="T"/> by its Id from store.
        /// </summary>
        /// <param name="id">Identifier. Can be empty string but not null.</param>
        /// <returns>
        /// Returns:
        /// - Bool that indicates if value has been found.
        /// - Value or default value of T if not found.
        /// - ConcurrencyKey associated with current version or <see cref="VersionKey.NonExisting"/> it not found.
        /// </returns>
        Task<(bool, T?, VersionKey)> TryGetAsync(string id);

        /// <summary>
        /// Sets <paramref name="value"/> with given <paramref name="id"/> to store.
        /// </summary>
        /// <param name="id">Identifier. Can be empty string but not null.</param>
        /// <param name="value">Value to store.</param>
        /// <param name="expectedVersion">Expected concurrency key of last known version or <see cref="VersionKey.NonExisting"/> for new items.</param>
        /// <returns>
        /// Returns:
        /// - Bool that indicates if <paramref name="expectedVersion"/> matches already stored values (optimistic concurrency check).
        /// - New <see cref="VersionKey"/> that identifies newly stored version.
        /// </returns>
        Task<(bool, VersionKey)> TrySetAsync(string id, T value, VersionKey expectedVersion);
    }
}
