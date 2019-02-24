using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class Collision : MonoBehaviour {

    [SerializeField] Melee meleeScript;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("sword collision");

        var hit = collision.gameObject;

        if (hit.layer == 9 && !hit.CompareTag("localPlayer"))//players are in layer9 && dont hit yourself
        {
            GameObject localPlayer = GameObject.FindGameObjectWithTag("localPlayer");
            float meleeDamage = localPlayer.GetComponent<Melee>().meleeDamage;

            localPlayer.GetComponent<Shoot>().CmdSendDamage(collision.gameObject, meleeDamage, 0);
        }
            
    }
}
