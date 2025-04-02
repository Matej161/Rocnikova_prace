using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DIsplayDaggerCount : MonoBehaviour
{
    public TMP_Text daggerCountText;
    private PlayerDaggerInventory daggerInventory;
    private Color originalColor;
    private bool isFlashing = false;

    void Start()
    {
        daggerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDaggerInventory>();
        originalColor = daggerCountText.color;
        UpdateDaggerCountUI();
    }

    void Update()
    {
        UpdateDaggerCountUI();
    }

    void UpdateDaggerCountUI()
    {
        if (daggerCountText != null && daggerInventory != null)
        {
            daggerCountText.text = daggerInventory.daggerCount.ToString();
        }
    }
    public void FlashRed()
    {
        if (!isFlashing)
        {
            StartCoroutine(FlashRedCoroutine());
        }
    }

    private IEnumerator FlashRedCoroutine()
    {
        isFlashing = true;
        daggerCountText.color = Color.red;
        yield return new WaitForSeconds(.3f);
        daggerCountText.color = originalColor;
        yield return new WaitForSeconds(.3f);
        isFlashing = false;
    }
}