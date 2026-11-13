using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class StartupWarning
{
    private const string PrefKey = "StartupWarningShown";

    static StartupWarning()
    {
        EditorApplication.delayCall += ShowDialogIfNeeded;
    }

    [MenuItem("Project/Show Important Notice")]
    public static void ShowDialogManual()
    {
        EditorPrefs.DeleteKey(PrefKey);
        ShowDialog();
    }

    private static void ShowDialogIfNeeded()
    {
        if (EditorPrefs.GetBool(PrefKey, false))
            return;

        ShowDialog();
    }

    private static void ShowDialog()
    {
        EditorUtility.DisplayDialog(
            "Important Notice",
            "Anyone that claims that they have the original source code, claims to be us, apart of us, or claims to have internal testing or private builds of puffles are scammers.\n\n" +
            "If you have paid for this source code/project in some fashion, PLEASE dispute said charge with your card issuing provider IMMEDIATELY.\n\n" +
            "This project is free and open sourced at:\n" +
            "https://github.com/OpenCPIsland/CPI-Project\n\n" +
            "We will never ask you or charge you money for this project.",
            "Next"
        );

        if (EditorUtility.DisplayDialog(
            "Open Source Link",
            "Would you like to open the official GitHub page?",
            "Open GitHub",
            "Skip"
        ))
        {
            Application.OpenURL("https://github.com/OpenCPIsland/CPI-Project");
        }

        EditorUtility.DisplayDialog(
            "Important Notice",
            "We are not Disney and we do not own the rights to anything in this project.",
            "OK"
        );

        EditorPrefs.SetBool(PrefKey, true);
    }
}