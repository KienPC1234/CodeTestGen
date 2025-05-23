using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GenerativeAI.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace CodeTestGen
{
    public class UpdateManager
    {
        private readonly string updateUrl = "https://raw.githubusercontent.com/KienPC1234/CodeTestGen/refs/heads/master/CodeTestGen/Update/update.json";
        private readonly string schemaUrl = "https://raw.githubusercontent.com/KienPC1234/CodeTestGen/refs/heads/master/CodeTestGen/Update/Update_Schema.json";
        private readonly HttpClient httpClient;
        private readonly string currentVersion;

        public UpdateManager(string currentVersion)
        {
            this.currentVersion = currentVersion ?? throw new ArgumentNullException(nameof(currentVersion));
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "UpdateManager/1.0");
        }

        public async Task<UpdateInfo> GetLatestVersionAsync()
        {
            try
            {
                string json = await FetchUpdateJsonAsync();
                if (!await ValidateJsonSchemaAsync(json))
                {
                    throw new InvalidOperationException("Update JSON does not conform to the schema.");
                }

                var updateData = JsonConvert.DeserializeObject<UpdateData>(json);
                return updateData.LatestVersion;
            }
            catch (Exception ex)
            {
                throw new UpdateException("Failed to fetch latest version.", ex);
            }
        }

        public async Task<bool> IsUpdateAvailableAsync()
        {
            try
            {
                var latestVersion = await GetLatestVersionAsync();
                return CompareVersions(currentVersion, latestVersion.Version) < 0;
            }
            catch (Exception ex)
            {
                throw new UpdateException("Failed to check for updates.", ex);
            }
        }

        public async Task<UpdateData> GetUpdateHistoryAsync()
        {
            try
            {
                string json = await FetchUpdateJsonAsync();
                if (!await ValidateJsonSchemaAsync(json))
                {
                    throw new InvalidOperationException("Update JSON does not conform to the schema.");
                }

                return JsonConvert.DeserializeObject<UpdateData>(json);
            }
            catch (Exception ex)
            {
                throw new UpdateException("Failed to fetch update history.", ex);
            }
        }

        private async Task<string> FetchUpdateJsonAsync()
        {
            HttpResponseMessage response = await httpClient.GetAsync(updateUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<bool> ValidateJsonSchemaAsync(string json)
        {
            try
            {
                string schemaJson = await httpClient.GetStringAsync(schemaUrl);
                JSchema schema = JSchema.Parse(schemaJson);
                JObject jObject = JObject.Parse(json);
                return jObject.IsValid(schema);
            }
            catch (Exception ex)
            {
                throw new UpdateException("Failed to validate JSON schema.", ex);
            }
        }

        private int CompareVersions(string current, string latest)
        {
            if (string.IsNullOrEmpty(current) || string.IsNullOrEmpty(latest))
                return -1;

            var currentParts = current.Split('.').Select(int.Parse).ToArray();
            var latestParts = latest.Split('.').Select(int.Parse).ToArray();

            for (int i = 0; i < Math.Min(currentParts.Length, latestParts.Length); i++)
            {
                if (currentParts[i] < latestParts[i])
                    return -1;
                if (currentParts[i] > latestParts[i])
                    return 1;
            }

            return currentParts.Length < latestParts.Length ? -1 : 0;
        }
    }

    public class UpdateData
    {
        [JsonProperty("latest_version", Required = Required.Always)]
        public UpdateInfo LatestVersion { get; set; }

        [JsonProperty("history", Required = Required.Always)]
        public UpdateInfo[] History { get; set; }
    }

    public class UpdateInfo
    {
        [JsonProperty("version", Required = Required.Always)]
        public string Version { get; set; }

        [JsonProperty("change_logs", Required = Required.Always)]
        public string ChangeLogs { get; set; }

        [JsonProperty("download_link", Required = Required.Always)]
        public string DownloadLink { get; set; }

        [JsonProperty("release_date", Required = Required.Always)]
        public DateTime ReleaseDate { get; set; }
    }

    public class UpdateException : Exception
    {
        public UpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}