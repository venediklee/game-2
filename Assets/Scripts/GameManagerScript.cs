using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    private void Start()
    {
        //this is to stop pickUp objects from colliding with bullets and players
        Physics2D.IgnoreLayerCollision(9, 11, true);//layer 9:player ,,,, layer 11 pickUp
        Physics2D.IgnoreLayerCollision(11, 12, true);//layer 11 pickUp,,, layer 12 bullets
        Physics2D.IgnoreLayerCollision(12, 12, true);//layer 12 bullets
        Physics2D.IgnoreLayerCollision(12, 15, true);//layer 12 bullets,,, layer 15 window
    }

    /// <summary>
    /// used for exiting game through UI
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
