using UnityEngine;
using TMPro;

/// <summary>
/// Test build'lerinde ekranda versiyon bilgisi gösterir.
/// Herhangi bir UI Canvas'a ekleyin. Release build'de otomatik gizlenir.
/// </summary>
public class VersionDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI versionText;
    [SerializeField] private bool showInRelease = false;

    private void Start()
    {
        if (versionText == null)
        {
            versionText = GetComponent<TextMeshProUGUI>();
        }

        if (versionText != null)
        {
            string version = Application.version;
            string buildType = Debug.isDebugBuild ? "TEST" : "RELEASE";
            versionText.text = $"v{version} ({buildType})";

            // Release build'de gizle (istenirse)
            if (!Debug.isDebugBuild && !showInRelease)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
