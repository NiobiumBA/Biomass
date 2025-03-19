using System;
using UnityEngine;

public class ChildSwitcher : MonoBehaviour
{
    public void SetChild(Transform child)
    {
        bool found = false;

        foreach (Transform currentChild in transform)
        {
            bool active = currentChild == child;

            if (active)
                found = true;

            currentChild.gameObject.SetActive(active);
        }

        if (!found && child != null)
            throw new ArgumentException($"Could not found a child {child.name}", nameof(child));
    }
}
