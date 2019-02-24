using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    [SerializeField] Camera playerCamera;

    void Update()
    {
        transform.LookAt(playerCamera.transform);
    }
}
