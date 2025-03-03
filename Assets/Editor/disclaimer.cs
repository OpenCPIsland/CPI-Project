using UnityEngine;
using UnityEditor;

public class DisclaimerWindow : EditorWindow
{
    private static bool dontShowAgain = false;

    // Create a custom menu item under "Legal" -> "Project Disclaimer"
    [MenuItem("Project/Legal/Project Disclaimer")]
    public static void ShowWindow()
    {
        // Opens a new window with the specified title
        DisclaimerWindow window = GetWindow<DisclaimerWindow>("Project Disclaimer");

        // Set the minimum size of the window to 800x600
        window.minSize = new Vector2(800, 600);
    }

    // Automatically show disclaimer on first use, unless user opts out
    [InitializeOnLoadMethod]
    private static void InitializeOnFirstUse()
    {
        // Check if the preference "showDisclaimer" exists and is set to false
        if (!EditorPrefs.HasKey("showDisclaimer") || EditorPrefs.GetBool("showDisclaimer", true))
        {
            ShowWindow();
        }
    }

    // This is where the disclaimer message will be drawn
    private void OnGUI()
    {
        // Set the title and add some padding
        GUILayout.Label("Disclaimer", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // The disclaimer message text
        string disclaimerText = "I hereby claim zero copyrights for the fan-made recreation of Disney's 'Club Penguin Island'.\n\n" +
                                "This project is a non-commercial, fan-made endeavor created solely for archival and educational purposes only.\n\n" +
                                "No profit is being made from this project, and all rights to 'Club Penguin Island' and related assets belong to Disney or their respective owners.\n\n" +
                                "I do not own any part of 'Club Penguin Island', including but not limited to the art, code, assets, music, fonts, models, or any other materials used in this fan-made recreation.\n\n" +
                                "I am not affiliated with Disney or any other company in any way, shape, or form.\n\n" +
                                "This fan-made recreation is intended to preserve the game for historical reference, archival, and educational purposes only.\n\n" +
                                "I do not want any legal trouble, and if you are a copyright holder and believe this project infringes upon your rights, " +
                                "please contact me and I will promptly remove any related material upon request.";

        // Display the message as non-editable text
        GUILayout.TextArea(disclaimerText, GUILayout.Height(240), GUILayout.ExpandWidth(true));

        // Add an option to not show the disclaimer window again
        dontShowAgain = GUILayout.Toggle(dontShowAgain, "Don't show this disclaimer again");

        // Add a button to close the window
        if (GUILayout.Button("Close"))
        {
            // Save the preference based on user's choice
            EditorPrefs.SetBool("showDisclaimer", !dontShowAgain);

            // Close the window cleanly
            Close();
        }
    }

    // Ensure the window can close properly
    private void OnDestroy()
    {
        // Make sure preferences are saved before the window is destroyed
        EditorPrefs.SetBool("showDisclaimer", !dontShowAgain);
    }
}
