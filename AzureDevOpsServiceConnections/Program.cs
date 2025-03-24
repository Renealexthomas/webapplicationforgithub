using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var serviceConnections = await GetAzureDevOpsServiceConnections(
                "ZRiVV08czdhikQsl6WYn5ZBjFqWwThDPAjgLJ6VSAWXF5vcNP9CGJQQJ99BAACAAAAAhG3NWAAASAZDOFtMJ",
                "vstsustglobal",
                "AzureDevOpsServiceConnections"
            );

            foreach (var connection in serviceConnections)
            {
                Console.WriteLine($"Service Connection Name: {connection.Name}");
                // You can add more fields to log here as needed
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public static async Task<ServiceConnection[]> GetAzureDevOpsServiceConnections(
        string personalAccessToken,
        string organization,
        string projectName)
    {
        var url = $"https://dev.azure.com/vstsustglobal/_apis/projects?api-version=7.1-preview.1";

        using (var client = new HttpClient())
        {
            // Set the Authorization header with the personal access token
            var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalAccessToken}"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {authHeader}");
            client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");

            // Send the GET request
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP error! Status: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var wrapper = JsonConvert.DeserializeObject<Wrapper>(responseContent);

            return wrapper.Value;
        }
    }

    // Define the necessary classes based on your TypeScript interfaces
    public class Wrapper
    {
        public int Count { get; set; }
        public ServiceConnection[] Value { get; set; }
    }

    public class ServiceConnection
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsOutdated { get; set; }
        public bool IsReady { get; set; }
        public bool IsShared { get; set; }
        public string Owner { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        // Add other fields if necessary based on the full interface
    }
}
