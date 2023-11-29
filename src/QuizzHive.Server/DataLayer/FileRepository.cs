
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace QuizzHive.Server.DataLayer
{
    public class FileRepository<T> : IRepository<T>
    {
        string BuildPath(string id) => $"bin/tmp.data-{typeof(T).Name}-{id}.json";

        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
        static Dictionary<string, VersionKey> versions = new(StringComparer.OrdinalIgnoreCase);

        readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public async Task<(bool, T?, VersionKey)> TryGetAsync(string id)
        {
            semaphoreSlim.Wait();
            try
            {
                string path = BuildPath(id);
                string json;
                try
                {
                    json = await File.ReadAllTextAsync(path);
                }
                catch (FileNotFoundException)
                {
                    return (false, default, VersionKey.NonExisting);
                }

                T value = JsonConvert.DeserializeObject<T>(json, serializerSettings) ?? throw new InvalidOperationException();

                if(!versions.TryGetValue(id, out VersionKey version))
                {
                    versions.Add(id, (version = VersionKey.NonExisting));
                }

                return (true, value, version);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task<(bool, VersionKey)> TrySetAsync(string id, T value, VersionKey expectedVersion)
        {
            semaphoreSlim.Wait();
            try
            {
                string path = BuildPath(id);

                bool versionMatch = File.Exists(path) switch
                {
                    // new item with correct expected version
                    false when expectedVersion == VersionKey.NonExisting => true,
                    // existing item with correct expected version
                    true when versions.TryGetValue(id, out VersionKey currentVersion) && currentVersion == expectedVersion => true,
                    // wrong version
                    _ => false
                };

                if (!versionMatch)
                {
                    return (false, default);
                }

                // write
                string json = JsonConvert.SerializeObject(value, serializerSettings);
                await File.WriteAllTextAsync(path, json);

                // and store new version key
                var newVersion = VersionKey.NewUnique();
                versions[id] = newVersion;

                return (true, newVersion);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
