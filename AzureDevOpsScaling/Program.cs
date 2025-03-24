using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    private static async Task GetBuildData(string personalAccessToken, string organization, string projectName)
    {
        var url = $"https://dev.azure.com/vstsustglobal/_apis/projects?api-version=7.1-preview.1";
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalAccessToken}")));
            client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");

            try
            {
                var response = await client.GetStringAsync(url);
                dynamic buildData = JsonConvert.DeserializeObject(response);
                Console.WriteLine("Build Data:");
                Console.WriteLine(buildData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    static async Task Main(string[] args)
    {
        string personalAccessToken = "ZRiVV08czdhikQsl6WYn5ZBjFqWwThDPAjgLJ6VSAWXF5vcNP9CGJQQJ99BAACAAAAAhG3NWAAASAZDOFtMJ";
        string organization = "vstsustglobal";
        string projectName = "AzureDevOpsScaling";

        await GetBuildData(personalAccessToken, organization, projectName);
    }
}
