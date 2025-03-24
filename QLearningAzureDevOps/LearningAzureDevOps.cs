using System;
using System.Net.Http;
using System.Threading.Tasks;

public class AzureDevOpsAPI
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task FetchBuildData(string personalAccessToken, string organization, string projectName)
    {
        //var url = $"https://dev.azure.com/vstsustglobal/_apis/projects?api-version=7.1-preview.1";
      var url =$"https://dev.azure.com/vstsustglobal/USTC-ISXX-AP-00_Timesheet_project/_apis/build/builds?api-version=7.1-preview.6";
 //var url =$"https://dev.azure.com/vstsustglobal/USTC-ISXX-AP-00_Timesheet_project/_apis/api-version=7.1";
        // Set up the Authorization header with PAT
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(":" + personalAccessToken)));

        // Fetch the data from Azure DevOps API
        HttpResponseMessage response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Fetched Build Data: \n");
            Console.WriteLine(jsonResponse);
        }
        else
        {
            Console.WriteLine("Error fetching data: " + response.ReasonPhrase);
        }
    }
}
