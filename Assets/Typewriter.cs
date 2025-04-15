using TMPro;
using System.Collections;
using UnityEngine;

public class TypewriterText : MonoBehaviour
{
    public TMP_Text textComponent;
    public float delayBetweenChars = 0.05f;
    [TextArea] public string fullText;
    private Coroutine typingCoroutine;

    public Coroutine StartTyping()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText());
        return typingCoroutine;
    }

    public void ClearText()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        textComponent.text = "";
    }

    IEnumerator TypeText()
    {
        textComponent.text = "";
        for (int i = 0; i < fullText.Length; i++)
        {
            textComponent.text += fullText[i];
            yield return new WaitForSeconds(delayBetweenChars);
        }
    }
}