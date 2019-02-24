using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Visibility : MonoBehaviour {

    //change alpha of the spriteRenderer of gameObject(default=self) based on if player is in trigger or not
    [SerializeField] bool changeSelfGFX = true;//if true changes this gameobjects color--default
    [SerializeField] bool isGrandChild = false;//if true changes parents parent gameobjects color
                                               // if both are false changes parents gameobjects color


    bool changedAlpha = false;//gives the information if we alrady changed alpha or not

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("localPlayer"))
        {
            ChangeAlpha();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!changedAlpha && collision.CompareTag("localPlayer")) //make sure we changed visibility
        {
            ChangeAlpha();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (changedAlpha && collision.CompareTag("localPlayer")) //return visibility to its old form
        {
            ChangeAlpha();
        }
    }




    public void ChangeAlpha()
    {
        changedAlpha = !changedAlpha;
        //Debug.Log("changing color alpha of " + transform.name +"'s parent::"+ transform.parent.name);

        Color color;

        if (changeSelfGFX) color = transform.GetComponent<SpriteRenderer>().color;
        else if (isGrandChild) color = transform.parent.parent.GetComponent<SpriteRenderer>().color;//doorframes need addinitional parent step
        else color = transform.parent.GetComponent<SpriteRenderer>().color;//isChild

        if (color.a == 0.1f)//if the player is already in the room(player is leaving) increase alpha to 1 again
        {
            color.a = 1f;
        }
        else//if the player is not in the room(player is entering) decrease the alpha to 0.01f
        {
            color.a = 0.1f;
        }
        //Debug.Log("changed the color of" + transform.parent.GetComponent<SpriteRenderer>().name);

        if (changeSelfGFX) transform.GetComponent<SpriteRenderer>().color = color;
        else if (isGrandChild) transform.parent.parent.GetComponent<SpriteRenderer>().color = color;//doorframes need addinitional parent step
        else transform.parent.GetComponent<SpriteRenderer>().color = color;//isChild
    }
}
