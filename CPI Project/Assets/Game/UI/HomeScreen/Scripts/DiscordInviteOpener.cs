using System;
using UnityEngine;

public class DiscordInviteOpener : MonoBehaviour
{
    [SerializeField]
    private string inviteUrl = "https://discord.gg/jkWbd3uqTS";

    public void OpenDiscordInvite()
    {
        string webUrl = NormalizeWebInviteUrl(inviteUrl);
        if (webUrl == null)
        {
            Debug.LogError("Discord invite URL is empty or invalid.", this);
            return;
        }

        Application.OpenURL(webUrl);
    }

    private static string NormalizeWebInviteUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        url = url.Trim();

        if (url.StartsWith("discord://", StringComparison.OrdinalIgnoreCase))
        {
            string deepLinkPath = url.Substring("discord://".Length);
            const string invitePath = "/invite/";
            int invitePathIndex = deepLinkPath.IndexOf(invitePath, StringComparison.OrdinalIgnoreCase);
            string inviteCode = invitePathIndex >= 0
                ? LastPathSegment(deepLinkPath.Substring(invitePathIndex + invitePath.Length))
                : LastPathSegment(deepLinkPath);
            return string.IsNullOrWhiteSpace(inviteCode) ? null : "https://discord.gg/" + inviteCode;
        }

        if (!url.Contains("://") && !url.Contains("/"))
            url = "https://discord.gg/" + url;
        else if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            url = "https://" + url.Substring("http://".Length);

        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri) ||
            !uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(uri.Host))
        {
            return null;
        }

        return uri.AbsoluteUri;
    }

    private static string LastPathSegment(string value)
    {
        int end = value.IndexOfAny(new[] { '?', '#' });
        if (end >= 0)
            value = value.Substring(0, end);

        value = value.TrimEnd('/');
        int lastSlash = value.LastIndexOf('/');
        return lastSlash >= 0 ? value.Substring(lastSlash + 1) : value;
    }
}
