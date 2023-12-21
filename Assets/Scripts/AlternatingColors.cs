using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AlternatingColors : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float revealDelay = 2.0f; // Time delay between line reveals
    public bool startIntro = false; // Set this to true in the Inspector or elsewhere to start the intro

    void Start()
    {
        // Check if startIntro is true before starting the coroutine
        if (startIntro && isActiveAndEnabled)
        {
            StartCoroutine(RevealText());
        }
    }

    void Update()
    {
        if (Input.anyKeyDown && startIntro)
        {
            StopAllCoroutines();
            SceneManager.LoadScene("Forest");
        }
    }

    IEnumerator RevealText()
    {
        string[] lines = {
            "In a candy forest where Christmas was lost,",
            "Santa's sleigh broken, Rudolph's fate crossed.",
            "Tethers to mortality severed and torn,",
            "Eldritch Rudolph, in shadows, is born.",
            "Four sleigh parts scattered, survival at stake,",
            "Through sweet illusions, the terror awaits.",
            "In peppermint glades, where nightmares deploy,",
            "Escape the eldritch, recover the joy."
        };

        for (int i = 0; i < lines.Length; i++)
        {
            // Concatenate lines up to the current index
            string visibleText = string.Join("\n", lines, 0, i + 1);
            textMeshPro.text = visibleText;

            // Wait for the specified reveal delay
            yield return new WaitForSeconds(revealDelay);
        }

        SceneManager.LoadScene("Forest");
    }

    // Public method to set startIntro to true from other scripts
    public void StartIntroFromOtherScript()
    {
        startIntro = true;
        StartCoroutine(RevealText());
    }
}
