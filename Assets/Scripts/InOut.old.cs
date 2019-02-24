//////using System.Collections;
//////using System.Collections.Generic;
//////using UnityEngine;

////////add this script to all walls and doors
////////bug in unity:: you must use parent.getcomponent, NOT getcomponentinparent
////////put this script IN CHILD object of the object with gfx or select isGrandChild, changeSelfGFX
//////public class InOut : MonoBehaviour {

//////    //genereal idea is saving the direction of the player when entering the trigger collider
//////    //then checking if the player keeps going in a similar direction as before
//////    //if the player keeps going in a similar direction then change the visibility of indoors
//////    //else dont change shit

//////        //how to find if the player goes in a similar direction::
//////        //get the velocity.normalized when entering
//////        //get the velocity.normalized when exiting
//////        //find the angle between the 2 velocities if it is less than 90 degrees player keeps going in the same direction
//////                                                                //TODO (test) 90 degrees is subject to change

//////        //how to change visibility indoors::
//////        //add a sprite( or something similar) to the walls PARENT(which is the room)
//////        //change alpha of it ONLY FOR LOCALPLAYER etc.

//////    Vector2 velocityEnter, velocityExit;
//////    bool localPlayerEntering;

//////    float stayTimer = 0.04f;
//////    SkillManagerScript skillManager;

//////    [SerializeField] bool isGrandChild, changeSelfGFX, activateWhenStaying;
//////    bool changeNow = false;//used for changing alpha when activateWhenStaying is active
//////    [HideInInspector] public bool playerNearWindow;//used for determining whether the player is near the window or not;

//////    private void OnTriggerEnter2D(Collider2D collision)
//////    {
//////        if (collision.CompareTag("localPlayer"))
//////        {
//////            skillManager = collision.GetComponent<SkillManagerScript>();

//////            Debug.Log("onTriggerEnter2D triggered @InOut of " + transform.name);
//////            velocityEnter = collision.GetComponent<Rigidbody2D>().velocity;
//////            localPlayerEntering = true;
//////            stayTimer = 0.04f;
//////            changeNow = !changeNow;
//////        }
//////        else localPlayerEntering = false;
//////    }

//////    private void OnTriggerStay2D(Collider2D collision)
//////    {
//////        if(collision.CompareTag("localPlayer"))
//////        {
//////            stayTimer -= Time.fixedDeltaTime;
//////            if (activateWhenStaying && changeNow)
//////            {
//////                Debug.Log("changing alpha@ ontriggerstay in activateWhenStaying");
//////                stayTimer = 0.04f;
//////                changeNow = !changeNow;
//////                ChangeAlpha();
//////            }
//////        }
//////    }

//////    private void OnTriggerExit2D(Collider2D collision)
//////    {
//////        if(activateWhenStaying && collision.CompareTag("localPlayer"))//used for bushes etc.
//////        {
//////            //changeNow = !changeNow;
//////            ChangeAlpha();
//////            localPlayerEntering = false;
//////        }

//////        else if (stayTimer > 0)
//////        {
//////            Debug.Log("player passed too quickly from trigger");
//////            return;
//////        }

//////        //Debug.Log("onTriggerExit2D triggered @InOut of " + transform.name);
//////        else if (localPlayerEntering && collision.CompareTag("localPlayer") && !playerNearWindow)// used for buildings etc.
//////        {
//////            velocityExit = collision.GetComponent<Rigidbody2D>().velocity;
//////            Debug.Log("angle between entrance & exit :: " + Vector2.Angle(velocityEnter, velocityExit));
//////            if (skillManager.inLeap ||   Vector2.Angle(velocityEnter, velocityExit) < 90)//change visibility
//////            {
//////                ChangeAlpha();
//////            }

//////            localPlayerEntering = false;
//////        }
//////    }


//////    public void ChangeAlpha()
//////    {
//////        //leaping gives 90* excess angle

//////        //Debug.Log("changing color alpha of " + transform.name +"'s parent::"+ transform.parent.name);

//////        Color color;

//////        if (changeSelfGFX) color = transform.GetComponent<SpriteRenderer>().color;
//////        else if (isGrandChild) color = transform.parent.parent.GetComponent<SpriteRenderer>().color;//doorframes need addinitional parent step
//////        else color = transform.parent.GetComponent<SpriteRenderer>().color;//isChild

//////        if (color.a == 0.1f)//if the player is already in the room(player is leaving) increase alpha to 1 again
//////        {
//////            color.a = 1f;
//////        }
//////        else//if the player is not in the room(player is entering) decrease the alpha to 0.01f
//////        {
//////            color.a = 0.1f;
//////        }
//////        //Debug.Log("changed the color of" + transform.parent.GetComponent<SpriteRenderer>().name);

//////        if (changeSelfGFX) transform.GetComponent<SpriteRenderer>().color = color;
//////        else if (isGrandChild) transform.parent.parent.GetComponent<SpriteRenderer>().color = color;//doorframes need addinitional parent step
//////        else transform.parent.GetComponent<SpriteRenderer>().color = color;//isChild
//////    }
//////}
