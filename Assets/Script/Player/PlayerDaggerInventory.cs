using UnityEngine;
using System.Collections;

public class PlayerDaggerInventory : MonoBehaviour
{
    public int daggerCount = 0;

    public void AddDagger()
    {
        daggerCount++;
    }

    public void RemoveDagger()
    {
        daggerCount--;
    }

    /*public bool UseDagger()
    {
        if (daggerCount > 0)
        {
            daggerCount--;
            return true;
        }
        return false;
    }*/
}
