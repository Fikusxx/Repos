using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControllerManager : SingletonMonobehaviour<SceneControllerManager>
{

    // Data
    private bool isFading = false; // self explanatory
    [SerializeField] private float fadeDuration = 1f; // how long it takes to fade in/out

    // References
    [SerializeField] private CanvasGroup faderCanvasGroup; // ref to Canvas so we can block/unblock raycasts 
    [SerializeField] private Image faderImage; // ref to image to change transpareny from 0 to 1 over fadeDuration time

    // Extra Data
    public SceneName startingSceneName; // set this in the Inspector from enums

    private IEnumerator Start()
    {
        // Set the initial alpha to start off with a black screen
        faderImage.color = new Color(0, 0, 0, 1);
        faderCanvasGroup.alpha = 1;

        // Start the first scene loading and wait for it to finish
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));

        // If this event has any subs - call it
        EventHandler.CallAfterSceneLoadedEvent();

        SaveLoadManager.Instance.RestoreCurrentStoreData();

        // Once the scene is finished loading, start fading in
        StartCoroutine(Fade(0f));
    }


    // This is the main external point of contact and influence from the rest of the project.
    // This will be called when the player wants to switch scenes
    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition) // spawnPos is the place where we want player to be when scene is loaded
    {
        // if a fade isnt happening then start fading and switch the scene
        if (isFading == false)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }

    // This is the coroutine where the "building blocks" of the script are put together
    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        // Call before fading black
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        // Start fading to black and wait for it to finish
        yield return StartCoroutine(Fade(1f));

        // Store scene data
        SaveLoadManager.Instance.StoreCurrentSceneData();

        // Set player's pos
        Player.Instance.transform.position = spawnPosition;

        // Call before scene unload
        EventHandler.CallBeforeSceneUnloadEvent();

        // Unload the current active scene
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        // Start loading the given scene and wait for it to finish
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        // Call after scene loaded
        EventHandler.CallAfterSceneLoadedEvent();

        // Restore scene data
        SaveLoadManager.Instance.RestoreCurrentStoreData();

        // Start fading back in and wait for it to finish before exiting the method
        yield return StartCoroutine(Fade(0f));

        // Call after fading back in is done
        EventHandler.CallAfterSceneLoadedFadeInEvent();
    }

    private IEnumerator Fade(float finalAlpha)
    {
        // Set isFading = true, so FadeAndSwitchScenes wont be called again
        isFading = true;

        // Make canvasGroup block raycasts, no more input/mouse over available
        faderCanvasGroup.blocksRaycasts = true;

        // Calculate how fast the CanvasGroup should fade based on it's current alpha, it's final alpha and how long it has to change between the two
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        // While CanvasGroup hasnt reached finalAlpha transparency
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            // move the alpha towards it's targetAlpha
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);

            // wait for a frame then continue
            yield return null;
        }

        // Set the bool to false, cause fading has finished
        isFading = false;

        // Stop the CanvasGroup from blocking raycasts
        faderCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        // Allow the given scene to load over several frames and add it to already loaded scenes
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Find the scene that was most recently loaded
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        // Set the newly loaded scene as the active scene (this marks it as the one to be unloaded next)
        SceneManager.SetActiveScene(newlyLoadedScene);
    }
}
