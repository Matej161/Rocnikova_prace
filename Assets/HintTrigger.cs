using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    public TMP_Text hintText;
    public EscapeTrigger cooldown;

    private bool triggerDisabled = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerDisabled) return;

        cooldown.EnableTriggerAfterDelay(3);
        if (other.CompareTag("Player"))
        {
            triggerDisabled = true;
            StartCoroutine(Hint());
        }
    }

    private IEnumerator Hint()
    {
        TypewriterText typewriter = hintText.GetComponent<TypewriterText>();
        if (typewriter == null) typewriter = hintText.AddComponent<TypewriterText>();

        typewriter.fullText = "Press F to shoot";
        typewriter.StartTyping();

        yield return new WaitForSeconds(3f);

        hintText.text = "";
        Destroy(gameObject);
    }

}
