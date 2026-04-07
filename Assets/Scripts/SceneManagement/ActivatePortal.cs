using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalActivator : MonoBehaviour
{
    [SerializeField] private GameObject portal;
    [SerializeField] private GameObject boss;

    private void Start()
    {
        if (portal != null)
            portal.SetActive(false);
    }

    private void Update()
    {
        if (boss == null && portal != null && !portal.activeSelf)
        {
            portal.SetActive(true);
            Debug.Log("Portal Activated!");
        }
    }
}