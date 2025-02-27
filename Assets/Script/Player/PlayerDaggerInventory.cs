using UnityEngine;
using System.Collections;

public class PlayerDaggerInventory : MonoBehaviour
{
    [SerializeField] public int daggerCount { get; private set; } = 0;

    public void AddDagger()
    {
        daggerCount++;
    }

    public bool UseDagger()
    {
        if (daggerCount > 0)
        {
            daggerCount--;
            return true;
        }
        return false;
    }
}
