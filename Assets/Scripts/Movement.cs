using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Movement : MonoBehaviour {

    float h, v;//used for movement directions
    [SerializeField] GameObject player;
    [SerializeField] float MAXSPEED=10f;
    private Rigidbody2D playerRigidbody;



    [SerializeField] Camera playerCam;
    [SerializeField] PlayerItems items;
    [SerializeField] SkillManagerScript skillManager;
    [SerializeField] AudioManager audioManager;
    [SerializeField] Shoot shootScript;

    Vector2 angleVector;//used for calculation player rotation
    float eulerZRotation;//used for calculating player rotation

    // Use this for initialization
    void Start ()
    {
        playerRigidbody = player.GetComponent<Rigidbody2D>();

	}
	
	// Update is called once per frame
	void Update ()
    {
        h = CrossPlatformInputManager.GetAxis("Horizontal");
        v = CrossPlatformInputManager.GetAxis("Vertical");
        items.moved = (h != 0f || v != 0f) ? true : false;

        if (h != 0f || v != 0f) audioManager.running = true;
        else audioManager.running = false;
    }


    
    private void FixedUpdate()
    {
        //set the movement
        //multiply the speed with 7*n% ;n being the number of points in fastMovement quirk
        playerRigidbody.velocity = new Vector2(h, v).normalized * MAXSPEED * 
            (100 + 7 * skillManager.quirkFastMovement) / 100;

        //get the angle vector
        angleVector = playerCam.ScreenToWorldPoint(Input.mousePosition) -
            shootScript.GetFirePoint().position;//playerRigidbody.transform.position;
        angleVector.Normalize();
        //find the Z angle
        eulerZRotation = Mathf.Atan2(angleVector.y, angleVector.x) * Mathf.Rad2Deg;
        //set the Z angle
        playerRigidbody.transform.rotation = Quaternion.Euler(0f, 0f, eulerZRotation - 90);
    }


    
}
