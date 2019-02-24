using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


//we do this so local player doesnt control other players too
public class PlayerSetup : NetworkBehaviour {

    [SerializeField] Behaviour[] whatToDisable;//scripts to disable on non local players

    [SerializeField] GameObject[] whatToDisable_NonBehaviour;//non script objects to disable on non local players

    Camera sceneCamera;

    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer) gameObject.tag = "localPlayer";
    }


    private void Start()
    {
        if (!isLocalPlayer)
        {
            foreach(Behaviour behave in whatToDisable)
            {
                behave.enabled = false;
            }
            foreach (GameObject obj in whatToDisable_NonBehaviour)
            {
                obj.SetActive(false);
            }
            gameObject.tag = "RemotePlayer";
        }
        else//disable main(scene) camera(its gameobject)
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null) Camera.main.gameObject.SetActive(false);
        }

        RegisterPlayer();
    }
    void RegisterPlayer()
    {
        //the id of players
        string ID = "Player" + gameObject.GetComponent<NetworkIdentity>().netId;
        transform.name = ID;
    }

    private void OnDisable()
    {
        if (sceneCamera != null) sceneCamera.gameObject.SetActive(true);
    }
}
