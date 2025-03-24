using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace AzureDevOpsMetrics
{
    class Program
    {
        private static readonly string organization = "vstsustglobal";
        private static readonly string project = "AzureDevOpsMetrics";
        private static readonly string pat = "37Se7XVPraDGp9F4cc2m1CZHnpqESSIW2McdX4wSlN8s9zjFP6bZJQQJ99BAACAAAAAhG3NWAAASAZDO3yU1";
        // Your Personal Access Token (PAT)
        private static readonly string apiVersion = "5.0"; // Azure DevOps API version

        static async Task Main(string[] args)
        {
            // Setup HttpClient
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Set authorization header (Base64-encoded PAT)
            var byteArray = Encoding.ASCII.GetBytes($":{pat}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            // Call the API to get work items
            var workItems = await GetWorkItems(client);

            // Output the result
            Console.WriteLine("Work Items:");
            foreach (var workItem in workItems)
            {
                Console.WriteLine($"ID: {workItem.Id}, Title: {workItem.Title}, State: {workItem.State}");
            }
        }

        // Method to get work items from Azure DevOps
        public static async Task<List<WorkItem>> GetWorkItems(HttpClient client)
        {
            try
            {
                var url = $"https://dev.azure.com/vstsustglobal/AzureDevOpsMetrics/_apis/teams?api-version=5.0-preview.2";
                var wiqlQuery = new
                {
                    query = "SELECT [System.Id], [System.Title], [System.State] FROM WorkItems WHERE [System.State] <> 'Closed' ORDER BY [System.Id] ASC"
                };
                var content = new StringContent(JsonConvert.SerializeObject(wiqlQuery), Encoding.UTF8, "application/json");

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var workItemsResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    var workItems = new List<WorkItem>();

                    // Process each work item
                    foreach (var item in workItemsResponse.value)
                    {
                        workItems.Add(new WorkItem
                        {
                            Id = item.id,
                            Title = item.fields["System.Title"],
                            State = item.fields["System.State"],
                           // AssignedTo = item.fields["System.AssignedTo"]?.displayName ?? "Unassigned"
                        });
                    }

                    return workItems;
                    Console.WriteLine(responseContent);
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return new List<WorkItem>();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Exception: {ex.Message}");
                return new List<WorkItem>();
            }

        }

    }

    // Class to represent the WorkItem data
    public class WorkItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string State { get; set; }
        //public string AssignedTo { get; set; } 
    }
}

