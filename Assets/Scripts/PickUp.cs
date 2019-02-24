using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//attach this to pickable objects 
// pickable object MUST HAVE COLLIDERS
[RequireComponent(typeof(Collider))]
public class PickUp : NetworkBehaviour {


    GameObject player;// the player
    InputManagerScript inputManager;



    [ClientRpc]
    public void RpcPickedUp()
    {
        this.gameObject.SetActive(false);
    }
    

    //##################### PICKUP INTERACTIONS etc. ######################//
    private void OnMouseEnter()
    {
        //TODO MAYBE LATER:: highlight the item(maybe show circular area ?)

        player = GameObject.FindGameObjectWithTag("localPlayer");
        Debug.Log(player.tag + "'s mouse is on object:" + this.gameObject.name);

        //if (player.) Debug.Log("not local player"); 

        inputManager = player.GetComponent<InputManagerScript>();
        //mouse is on pickable
        inputManager.SetItemOnMouse(this.gameObject);
    }

    private void OnMouseExit()
    {
        // TODO MAYBE LATER:: de-highlight the item
        //mouse is not on pickable anymore
        inputManager.ClearItemOnMouse();
    }
}
