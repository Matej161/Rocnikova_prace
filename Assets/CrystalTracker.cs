using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;


public class CrystalTracker : MonoBehaviour
{
    public Image[] crystalIcons; 
    public Color[] crystalColors; 
    public TMP_Text hintText;

    public int collectedCrystals = 0;

    public void CollectCrystal(Color collectedColor)
    {
        if (collectedCrystals < crystalIcons.Length)
        {
            Color opaqueColor = collectedColor;
            opaqueColor.a = 1f;

            crystalIcons[collectedCrystals].color = opaqueColor;

            collectedCrystals++;
        }
        if (collectedCrystals == 4)
        {
            StartCoroutine(GetBackUp());
        }
    }

    IEnumerator GetBackUp()
    {
        TypewriterText typewriter = hintText.GetComponent<TypewriterText>();
        if (typewriter == null) typewriter = hintText.AddComponent<TypewriterText>();
        typewriter.fullText = "You collected all the sufficient crystals.\nGet back up to resonate.";
        typewriter.StartTyping();
        yield return new WaitForSeconds(3f);
        hintText.text = "";
    }

    public void ResetCrystals()
    {
        collectedCrystals = 0;
        foreach (Image icon in crystalIcons)
        {
            icon.color = Color.gray;
        }
    }
}