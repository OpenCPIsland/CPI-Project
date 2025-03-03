using UnityEditor;
using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;

public class GitHubCommitViewer : EditorWindow
{
    private string repositoryOwner;
    private string repositoryName;
    private string commitsData = "";
    private string errorMessage = "";
    private Vector2 scrollPosition; // Scroll position for the commit list

    private const string ownerKey = "GitHubCommitViewer_RepoOwner";
    private const string repoKey = "GitHubCommitViewer_RepoName";

    [MenuItem("Project/GitHub Commit Viewer")]
    public static void ShowWindow()
    {
        GetWindow<GitHubCommitViewer>("GitHub Commit Viewer");
    }

    private void OnEnable()
    {
        // Load saved repo owner and name
        repositoryOwner = EditorPrefs.GetString(ownerKey, "CP-Island-Localhost-Server");
        repositoryName = EditorPrefs.GetString(repoKey, "CPI-Project");
    }

    private void OnGUI()
    {
        GUILayout.Label("GitHub Commit History", EditorStyles.boldLabel);

        // Input fields for repository owner and name
        repositoryOwner = EditorGUILayout.TextField("Repository Owner", repositoryOwner);
        repositoryName = EditorGUILayout.TextField("Repository Name", repositoryName);

        // Save the inputs when they change
        EditorPrefs.SetString(ownerKey, repositoryOwner);
        EditorPrefs.SetString(repoKey, repositoryName);

        // Display error message if the repo is not found
        if (!string.IsNullOrEmpty(errorMessage))
        {
            EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
        }

        // Add some space before the "Fetch Recent Commits" button
        GUILayout.FlexibleSpace();

        // Horizontal centering for "Fetch Recent Commits" button
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Fetch Recent Commits", GUILayout.Width(200)))
        {
            FetchCommitsAsync();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Add FlexibleSpace after the button to center it vertically
        GUILayout.FlexibleSpace();

        // Scrollable area for commit list (vertical and horizontal scrolling)
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true, GUILayout.Height(400));
        GUILayout.TextArea(commitsData, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUILayout.EndScrollView();

        // Horizontal centering for the "Open GitHub Repo" button
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Open GitHub Repo", GUILayout.Width(200)))
        {
            OpenGitHubRepo();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Horizontal centering for the "Open GitHub Commits Page" button
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Open GitHub Commits Page", GUILayout.Width(200)))
        {
            OpenGitHubCommitsPage();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
    }

    // Separate async method to call FetchCommits and update commitsData
    private async void FetchCommitsAsync()
    {
        commitsData = await FetchCommits(repositoryOwner, repositoryName);
    }

    // Function to fetch commits with error handling
    private async Task<string> FetchCommits(string owner, string repo)
    {
        using (HttpClient client = new HttpClient())
        {
            string url = $"https://api.github.com/repos/{owner}/{repo}/commits";
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Unity-GitHub-Viewer");

            try
            {
                var response = await client.GetAsync(url);

                // Check if the repository exists (status code 200)
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    JArray commits = JArray.Parse(content);

                    string commitHistory = "";
                    foreach (var commit in commits)
                    {
                        string message = commit["commit"]["message"].ToString();
                        string author = commit["commit"]["author"]["name"].ToString();
                        string isoDate = commit["commit"]["author"]["date"].ToString();

                        // Convert ISO date string to DateTime in UTC
                        DateTime utcDateTime = DateTime.Parse(isoDate).ToUniversalTime();

                        // Convert to Central Time (adjust based on whether DST is active)
                        TimeZoneInfo centralZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                        DateTime centralTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, centralZone);

                        // Manually adjust for Daylight Saving Time (DST)
                        if (centralZone.IsDaylightSavingTime(centralTime))
                        {
                            centralTime = centralTime.AddHours(-5); // CDT (UTC - 5)
                        }
                        else
                        {
                            centralTime = centralTime.AddHours(-6); // CST (UTC - 6)
                        }

                        // Format date with "Central" label and 12-hour AM/PM format
                        string formattedDate = centralTime.ToString("MM-dd-yyyy hh:mm tt") + " Central";

                        // Add the commit info to the history
                        commitHistory += $"Author: {author}\nDate: {formattedDate}\nMessage: {message}\n\n";
                    }
                    errorMessage = ""; // Clear error message if successful
                    return commitHistory;
                }
                else
                {
                    errorMessage = "The repository could not be found. Please check the owner and repository names, and try again.";
                    return ""; // Return empty string if error
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request error: {e.Message}");
                errorMessage = "The repository could not be found. Please check the owner and repository names, and try again.";
                return ""; // Return empty string if error occurs
            }
        }
    }

    // Function to open the GitHub repository page in a web browser
    private void OpenGitHubRepo()
    {
        string url = $"https://github.com/{repositoryOwner}/{repositoryName}";
        Application.OpenURL(url); // Open the GitHub repository in the default browser
    }

    // Function to open the GitHub commits page in a web browser
    private void OpenGitHubCommitsPage()
    {
        string url = $"https://github.com/{repositoryOwner}/{repositoryName}/commits";
        Application.OpenURL(url); // Open the GitHub commits page in the default browser
    }
}
