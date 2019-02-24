using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowVisibility : MonoBehaviour {


    //activate and deactivate changeAlpha function from visibility script that resides in parent

    bool changedAlpha = false;//gives the information if we alrady changed alpha or not
    Visibility visibility;//visibility script of parent

    private void Start()
    {
        visibility = this.gameObject.GetComponentInParent<Visibility>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("localPlayer"))
        {
            changedAlpha = true;
            visibility.ChangeAlpha();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!changedAlpha && collision.CompareTag("localPlayer"))
        {
            changedAlpha = true;
            visibility.ChangeAlpha();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("localPlayer"))
        {
            changedAlpha = false;
            visibility.ChangeAlpha();
        }
    }
}
