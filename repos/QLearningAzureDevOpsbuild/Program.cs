using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

class QLearningAzureDevOps
{
    static double alpha = 0.1;  // Learning rate
    static double gamma = 0.9;  // Discount factor
    static int numStates = 5;   // Number of states (e.g., different build durations)
    static int numActions = 2;  // Actions: scale up or scale down
    static double[,] Q = new double[numStates, numActions]; // Q-table
    static Random rand = new Random();
    static double[] rewards = { 0, 0, 1, 0, -1 }; // Example: Reward for states (faster build times)
    static List<int> buildDurations = new List<int>(); // To store build durations

    static async Task Main(string[] args)
    {
        string personalAccessToken = "ZRiVV08czdhikQsl6WYn5ZBjFqWwThDPAjgLJ6VSAWXF5vcNP9CGJQQJ99BAACAAAAAhG3NWAAASAZDOFtMJ";  // Replace with your PAT
        string organization = "vstsustglobal"; // Replace with your organization
        string projectName = "https://dev.azure.com/vstsustglobal/USTC-ISXX-AP-00_Timesheet_project/_apis/build/builds?api-version=7.1-preview.6"; // Replace with your project name
        
        // Fetch build data from Azure DevOps API
        await AzureDevOpsAPI.FetchBuildData(personalAccessToken, organization, projectName);

        // Based on the build data, let's say we classify build durations into states
        for (int i = 0; i < buildDurations.Count; i++)
        {
            // For simplicity, we'll map build duration to states
            int state = ClassifyBuildDuration(buildDurations[i]);

            // Number of episodes for training
            int episodes = 1000;
            for (int episode = 0; episode < episodes; episode++)
            {
                int currentState = state;  // Start from current state based on build data
                bool done = false;

                // Run a single episode
                while (!done)
                {
                    int action = ChooseAction(currentState);  // Choose an action
                    int nextState = TakeAction(currentState, action);  // Transition to next state
                    double reward = rewards[nextState];  // Get reward for this transition

                    // Update Q-table using Q-learning formula
                    UpdateQ(currentState, action, reward, nextState);

                    currentState = nextState;  // Move to next state
                    done = (currentState == 4);  // If we reach the goal state
                }
            }
        }

        // Print the final Q-table
        Console.WriteLine("Final Q-table:");
        PrintQTable();
    }

    static int ClassifyBuildDuration(int buildDuration)
    {
        // Example logic for classifying build durations into states (you may need a more advanced classification logic)
        if (buildDuration < 5) return 0; // Fast builds
        if (buildDuration < 10) return 1; // Medium builds
        if (buildDuration < 15) return 2; // Longer builds
        if (buildDuration < 20) return 3; // Very long builds
        return 4; // Extremely long builds
    }

    static int ChooseAction(int state)
    {
        double epsilon = 0.1;  // 10% chance to explore
        if (rand.NextDouble() < epsilon)
        {
            return rand.Next(numActions);  // Random action (explore)
        }
        else
        {
            return Q[state, 0] > Q[state, 1] ? 0 : 1;  // Best action (exploit)
        }
    }

    static int TakeAction(int state, int action)
    {
        // Dummy transition logic for simplicity
        return (state + 1) % numStates;  // Transition to the next state
    }

    static void UpdateQ(int state, int action, double reward, int nextState)
    {
        double maxNextQ = Math.Max(Q[nextState, 0], Q[nextState, 1]);
        Q[state, action] = Q[state, action] + alpha * (reward + gamma * maxNextQ - Q[state, action]);
    }

    static void PrintQTable()
    {
        for (int i = 0; i < numStates; i++)
        {
            Console.Write($"State {i}: ");
            for (int j = 0; j < numActions; j++)
            {
                Console.Write($"{Q[i, j]:0.00} ");
            }
            Console.WriteLine("\n");
        }
    }
}

class AzureDevOpsAPI
{
    public static async Task FetchBuildData(string personalAccessToken, string organization, string projectName)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(":" + personalAccessToken)));

            // Define the URL for fetching build data
            string url =  $"https://dev.azure.com/vstsustglobal/USTC-ISXX-AP-00_Timesheet_project/_apis/build/builds?api-version=7.1-preview.6";

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var builds = JObject.Parse(responseBody)["value"];

                // Process the builds data here and update your states and rewards based on this
                foreach (var build in builds)
                {
                    var finishTime = build["finishTime"]?.ToString();
                    var status = build["status"]?.ToString();
                    var startTime = build["startTime"]?.ToString();

                    if (DateTime.TryParse(finishTime, out DateTime finishDate) && DateTime.TryParse(startTime, out DateTime startDate))
                    {
                        int buildDuration = (int)(finishDate - startDate).TotalMinutes;
                        QLearningAzureDevOps.buildDurations.Add(buildDuration);
                    }
                }

                Console.WriteLine($"Fetched {builds.Count()} builds from Azure DevOps.");
            }
            else
            {
                Console.WriteLine("Error fetching build data");
            }
        }
    }
}
