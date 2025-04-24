using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class EscapeTrigger : MonoBehaviour
{
    public CrystalTracker crystalTracker;
    public TMP_Text hintText;
    public Image whiteFade;
    public Transform tpPosition;
    public Health playerHealth;

    private bool deathTriggered = false;
    private bool triggerDisabled = false;

    [SerializeField] AudioClip endingSoundClip;
    [SerializeField] private float endingSoundVolume;

    private void Update()
    {
        if (playerHealth.dead && !deathTriggered)
        {
            deathTriggered = true;
            StartCoroutine(DeathScreen());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerDisabled) return;

        if (other.CompareTag("Player"))
        {
            triggerDisabled = true;
            StartCoroutine(EnableTriggerAfterDelay(12f));

            if (crystalTracker.collectedCrystals < 4)
            {
                StartCoroutine(ShowHint());
            }
            else
            {
                StartCoroutine(Escape());
            }
        }
    }

    IEnumerator ShowHint()
    {
        TypewriterText typewriter = hintText.GetComponent<TypewriterText>();
        if (typewriter == null) typewriter = hintText.AddComponent<TypewriterText>();
        typewriter.fullText = "There’s a pull toward the crystals.";
        typewriter.StartTyping();
        yield return new WaitForSeconds(3f);
        hintText.text = "";
        typewriter.fullText = "Collect the four.\nMaybe then… it will all make sense.";
        typewriter.StartTyping();
        yield return new WaitForSeconds(4f);
        hintText.text = "";
    }

    IEnumerator DeathScreen()
    {
        yield return new WaitForSeconds(1f);
        float fadeTime = 2f;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            whiteFade.color = new Color(0, 0, 0, Mathf.Lerp(0.5f, 1f, t / fadeTime));
            yield return null;
        }
        whiteFade.color = Color.black;
        yield return new WaitForSeconds(2f);

        TypewriterText typewriter = hintText.GetComponent<TypewriterText>();
        if (typewriter == null) typewriter = hintText.AddComponent<TypewriterText>();

        typewriter.fullText = "You died";
        typewriter.StartTyping(); 
        yield return new WaitForSeconds(2f); 

        hintText.text = "";
        typewriter.fullText = "You will never know";
        typewriter.StartTyping();
        yield return new WaitForSeconds(2f);

        hintText.text = "";
        SceneManager.LoadScene(0);
    }

    public IEnumerator Escape()
    {
        SoundFXManager.Instance.PlaySoundFXClip(endingSoundClip, transform, endingSoundVolume);
        //flash
        float pulseDuration = 2f;
        for (float t = 0; t < pulseDuration; t += Time.deltaTime)
        {
            whiteFade.color = new Color(1, 1, 1, Mathf.PingPong(t, 0.5f));
            yield return null;
        }

        float fadeTime = 2f;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            whiteFade.color = new Color(1, 1, 1, Mathf.Lerp(0.5f, 1f, t / fadeTime));
            yield return null;
        }
        whiteFade.color = Color.white;

        //teleport
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = tpPosition.position;
        yield return new WaitForSeconds(1f);

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            whiteFade.color = new Color(1, 1, 1, Mathf.Lerp(1f, 0f, t / fadeTime));
            yield return null;
        }
        whiteFade.color = new Color(1, 1, 1, 0);

        yield return new WaitForSeconds(2f);

        whiteFade.color = Color.black;

        yield return new WaitForSeconds(2f);

        TypewriterText typewriter = hintText.GetComponent<TypewriterText>();
        if (typewriter == null) typewriter = hintText.AddComponent<TypewriterText>();
        typewriter.fullText = "You remembered...";
        typewriter.StartTyping(); 

        yield return new WaitForSeconds(5f);
        typewriter.fullText = "There was never an exit";
        typewriter.StartTyping(); 

        yield return new WaitForSeconds(5f);

        Destroy(gameObject);

        SceneManager.LoadScene(0);
    }

    public IEnumerator EnableTriggerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        triggerDisabled = false;
    }
}