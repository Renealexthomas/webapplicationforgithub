using System.Threading.Tasks;
using System;

class QLearningAzureDevOps
{
    static double alpha = 0.1;  // Learning rate
    static double gamma = 0.9;  // Discount factor
    static int numStates = 5;   // Number of states (e.g., different build durations)
    static int numActions = 2;  // Actions: scale up or scale down
    static double[,] Q = new double[numStates, numActions]; // Q-table
    static Random rand = new Random();
    static double[] rewards = { 0, 0, 1, 0, -1 }; // Example: Reward for states (faster build times)

    static async Task Main(string[] args)
    {
        string personalAccessToken = "ZRiVV08czdhikQsl6WYn5ZBjFqWwThDPAjgLJ6VSAWXF5vcNP9CGJQQJ99BAACAAAAAhG3NWAAASAZDOFtMJ";
        string organization = "vstsustglobal";
      //  string projectName = "https://dev.azure.com/vstsustglobal/_apis/projects?api-version=7.1-preview.1";
string projectName= "https://dev.azure.com/vstsustglobal/USTC-ISXX-AP-00_Timesheet_project/_apis/build/builds?api-version=7.1-preview.6";
//string projectName ="https://dev.azure.com/vstsustglobal/USTC-ISXX-AP-00_Timesheet_project/api-version=7.1-preview.1";

        // Fetch build data from Azure DevOps API
        await AzureDevOpsAPI.FetchBuildData(personalAccessToken, organization, projectName);

        // Number of episodes for training
        int episodes = 1000;
        for (int episode = 0; episode < episodes; episode++)
        {
            int state = 0;  // Initial state (e.g., start with build state 0)
            bool done = false;

            while (!done)
            {
                int action = ChooseAction(state);  // Choose an action using epsilon-greedy
                int nextState = TakeAction(state, action);  // Get next state after taking action
                double reward = rewards[nextState];  // Reward for this transition

                // Update Q-table using the Q-learning formula
                UpdateQ(state, action, reward, nextState);

                state = nextState;  // Move to next state
                done = (state == 4);  // If we reach the goal state
            }
        }

        // Print the final Q-table
        Console.WriteLine("Final Q-table:");
        PrintQTable();
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
