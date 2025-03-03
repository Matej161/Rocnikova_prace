using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DIsplayDaggerCount : MonoBehaviour
{
    public TMP_Text daggerCountText;
    private PlayerDaggerInventory daggerInventory;

    void Start()
    {
        daggerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDaggerInventory>();
        UpdateDaggerCountUI();
    }

    // Update is called once per frame
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
}
