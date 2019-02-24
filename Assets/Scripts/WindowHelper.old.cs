//////using System.Collections;
//////using System.Collections.Generic;
//////using UnityEngine;

//////[RequireComponent(typeof(Collider2D))]//used for triggering window events
//////public class WindowHelper : MonoBehaviour {


//////    private void OnTriggerEnter2D(Collider2D collision)
//////    {
//////        if (collision.CompareTag("localPlayer"))
//////        {
//////            //change alpha of the roof and 
//////            //set playerNearWindow on nearby(that falls within the colliders area+some offset) walls

//////            //setting playerNearWindow
//////            Collider2D[] nearbyWalls = Physics2D.OverlapCircleAll
//////                (transform.position, gameObject.GetComponent<CapsuleCollider2D>().size[0] + 3, LayerMask.GetMask("wall"));

//////            int nearbyWallWithInOut = -1;
//////            for (int i = 0; i < nearbyWalls.Length; i++)
//////            {
//////                if (nearbyWalls[i].GetComponent<InOut>() != null)
//////                {
//////                    nearbyWalls[i].GetComponent<InOut>().playerNearWindow = true;
//////                    nearbyWallWithInOut = i;
//////                }
//////            }
//////            Debug.Log(nearbyWalls.Length);
//////            //change alpha of the roof
//////            if(nearbyWallWithInOut!=-1) nearbyWalls[nearbyWallWithInOut].GetComponent<InOut>().ChangeAlpha();

//////            //velocityEnter = collision.GetComponent<Rigidbody2D>().velocity;
//////        }
//////    }

//////    private void OnTriggerExit2D(Collider2D collision)
//////    {
//////        if (collision.CompareTag("localPlayer"))
//////        {
//////            //change the alpha of the roof if the player did not go inside the building
//////            //set playerNearWindow to false

//////            //setting playerNearWindow
//////            Collider2D[] nearbyWalls = Physics2D.OverlapCircleAll
//////                (transform.position, gameObject.GetComponent<CapsuleCollider2D>().size[0] + 3, LayerMask.GetMask("wall"));
//////            int nearbyWallWithInOut = -1;
//////            for (int i = 0; i < nearbyWalls.Length; i++)
//////            {
//////                if (nearbyWalls[i].GetComponent<InOut>() != null)
//////                {
//////                    nearbyWalls[i].GetComponent<InOut>().playerNearWindow = false;
//////                    nearbyWallWithInOut = i;
//////                }
//////            }
//////            if (nearbyWallWithInOut == -1) return;
//////            //determine if the player is inside or outside by checking the entry and exit velocities
//////            //velocityExit = collision.GetComponent<Rigidbody2D>().velocity;
//////            if (collision.GetComponent<SkillManagerScript>().inLeap)
//////            {
//////                //if (Vector2.Angle(velocityEnter, velocityExit) == 90)//change visibility
//////                    nearbyWalls[nearbyWallWithInOut].GetComponent<InOut>().ChangeAlpha();
//////            }
//////            else
//////            {
//////                //if(Vector2.Angle(velocityEnter, velocityExit) < 180)//change visibility
//////                    nearbyWalls[nearbyWallWithInOut].GetComponent<InOut>().ChangeAlpha();
//////            }
//////        }
//////    }
//////}
