using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//attach this script to DOORPIVOT only
public class DoorScript : NetworkBehaviour {

    Transform door;
    GameObject player;

    private void Start()
    {
        door = transform.GetChild(0);
    }

    public override void OnStartLocalPlayer()
    {
        player = GameObject.FindGameObjectWithTag("localPlayer");
    }

    public void DoorMovement(Transform playerPos)
    {
        Debug.Log("e pressed near door");
        RaycastHit2D hit = Physics2D.Raycast(playerPos.position,
        door.transform.position - playerPos.position, 5);

        if (hit == door)//open or close the door
        {
            Animator animator = GetComponent<Animator>();

            if (animator.GetBool("OpenInside") == true)//the door is opened through inside, close it
            {
                CmdAnimPlayer(gameObject, "OpenInside", false);
            }
            else if (animator.GetBool("OpenOutside") == true)//the door is opened through outside, close it
            {
                CmdAnimPlayer(gameObject, "OpenOutside", false);
            }
            else//the door is closed, check if player is inside or outside, open the door accordingly
            {
                // TODO ERROR LATER:: this will not function properly if the door is vertical
                //check if the player is inside by comparing the doors and doorKnobs y distances to player
                float doorDistance, doorKnobDistance;
                doorDistance = door.transform.position.y - playerPos.position.y;
                doorKnobDistance = door.transform.GetChild(0).transform.position.y - playerPos.position.y;

                if (Mathf.Abs(doorDistance) > Mathf.Abs(doorKnobDistance))//the player is outside
                {
                    //open the door through inside
                    CmdAnimPlayer(gameObject, "OpenInside", true);

                }
                else CmdAnimPlayer(gameObject, "OpenOutside", true);//player is inside open the door through outside
            }
        }
    }

    /// <summary>
    /// plays the animation of the attached to GameObject on all clients
    /// </summary>
    /// <param name="animator"> GameObject of the animator to use. </param>
    /// <param name="param"> parameter to change. </param>
    /// <param name="value"> value to set.</param>
    [Command]
    void CmdAnimPlayer(GameObject animator, string param, bool value)
    {
        RpcAnimPlayer(animator, param, value);
    }
    [ClientRpc]
    void RpcAnimPlayer(GameObject animator, string param, bool value)
    {
        animator.GetComponent<Animator>().SetBool(param, value);
    }

    

}
