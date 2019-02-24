using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {


    float myFireDamage;

    public GameObject localPlayer;

    private void OnCollisionEnter2D(Collision2D collision)
    {

        GameObject hit = collision.gameObject;
        if (hit.layer==9 && !hit.CompareTag("localPlayer"))//players are in 9'th layer 
        {
            localPlayer = GameObject.FindGameObjectWithTag("localPlayer");
            float damage= localPlayer.GetComponent<PlayerItems>().GetHeldGunCharacteristics()[1];
            int armorPiercing = localPlayer.GetComponent<SkillManagerScript>().quirkArmorPiercing;
            Debug.Log(hit.tag+" hit by "+ localPlayer.tag+" with "+ damage);
            //send hit information to the local player
            localPlayer.GetComponent<Shoot>().CmdSendDamage(hit, damage, armorPiercing);
        }

        //destroys the bullet after hitting something
        Destroy(this.gameObject);
    }



    //OnCollisionStay2D & OnCollisionExit2D ensure bullets destruction upon impact
    private void OnCollisionStay2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }


}
