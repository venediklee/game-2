using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;


public class InputManagerScript : NetworkBehaviour {

    [SerializeField] SkillManagerScript skillManager;
    [SerializeField] Shoot shootScript;
    [SerializeField] PlayerItems items;
    [SerializeField] Melee meleeScript;

    [SerializeField] GameObject player;// the player
    [SerializeField] Camera playerCam;

    [HideInInspector] public bool isMouseOnPickable;// used for picking objects
    [HideInInspector] public GameObject itemOnMouse;//used for identifying the object on mouse

    [SerializeField] Bullet bulletScript;
    [SerializeField] GameObject skillMenu;

    [SerializeField] GameObject mapCanvas;//canvas of big map(not minimap)
    [SerializeField] Camera mapCam;//cam of big map (not minimap)

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject [] otherMenus;

    private void Start()
    {
        //make cam & camvas's parents null to prevent rotation
        mapCam.transform.SetParent(null);
        mapCanvas.transform.SetParent(null);
        mapCam.transform.position = new Vector3(0, 0, -75);
    }

    void Update () {



        meleeScript.isMeleeAttacking = CrossPlatformInputManager.GetButtonDown("Fire2");
        items.attacked= CrossPlatformInputManager.GetButtonDown("Fire2");

        if (CrossPlatformInputManager.GetButton("Fire1"))//autofire
        {
            if(items.currentlyHeld<=2)//only guns autofire
            {
                shootScript.Fire();
            }
        }
        if(CrossPlatformInputManager.GetButtonDown("Fire1"))//single fire
        {
            //possible outcomes: shoot gun, use kit

            if(items.currentlyHeld<=3)//if currentlyHeld item is a gun
            {
                shootScript.Fire();//will shoot once
            }
            else//if the player is holding a kit
            {
                StartCoroutine(items.UseKit());
            }
        }
        
        if (Input.GetKeyDown("f"))//f key for invisibility
        {
            //if the player has no points in skillInvisibility
            if (skillManager.skillInvisibility == 0)
            {
                Debug.LogError("Skill Error: Player has no points in invisibility skill!");
            }
            else StartCoroutine(skillManager.SkillInvisibility(false));
            
        }

        if(Input.GetKeyDown("g")) //g key for leap
        {
            if (skillManager.skillLeap == 0) Debug.LogError
                     ("Skill Error: Player has no points in leap skill!");
            else skillManager.SkillLeap();
        }

        if(Input.GetKeyDown("v"))//v key for martial arts (right now it is invis+leap+4 slash)
        {
            StartCoroutine(skillManager.SkillMartialArt());
        }

        if(Input.GetKeyDown("r")) // r key for reload
        {
            shootScript.Reload();
        }

        
        if (Input.GetKeyDown("t")) // t key for dropping current item
        {
            //items.enableSelectGunIndicator(true);
            items.DropGun();
        }
        
        
        if (Input.GetKeyDown("e") && !isMouseOnPickable) //e for opening doors & using kits
        {
            //if player is near a door and looking at it DIRECTLY
            Transform playerPos = player.transform;

            Collider2D door = Physics2D.OverlapCircle(playerPos.position, 2, LayerMask.GetMask("door"));//doors are @ 13th layer

            //foreach(Collider2D dor in door)
            //{
            //    Debug.Log("name of the collider: " + dor.name);
            //    Debug.Log("layer of the collider: " + dor.gameObject.layer);
            //}
            if (door != null)
            {
                door.GetComponentInParent<DoorScript>().DoorMovement(playerPos);//doorScript is at DoorPivot which is the parent of door
            }

            else if (items.currentlyHeld == 4)// kits are @4
            {
                //use kit
                StartCoroutine(items.UseKit());
            }

            Debug.Log("e pressed & mouse not on pickable");
        }

        if(Input.GetKeyDown("e") && isMouseOnPickable)// e for picking up objects
        {
            Debug.Log("e pressed & mouse on pickable");
            //check distance
            float distance = Vector2.Distance(player.transform.position,
                playerCam.ScreenToWorldPoint(Input.mousePosition));
            if (distance <= 3f)
            {
                Debug.Log("picking item");
                //if inventory is empty put it to empty slot
                //else drop the current weapon & pick it up
                bool successfulPickUp = items.SetFirstEmptyGun(itemOnMouse);

                //disable the gun so others dont take it
                if (successfulPickUp)
                {
                    CmdPickedUp(itemOnMouse);
                    ClearItemOnMouse();
                } 
            }
        }

        if(Input.GetKeyDown("i"))// i key to open skillMenu
        {
            if (skillMenu.activeSelf) skillMenu.SetActive(false);
            else skillMenu.SetActive(true);
        }

        if(Input.GetKeyDown("m"))
        {
            if (mapCanvas.activeSelf)
            {
                mapCam.gameObject.SetActive(false);
                mapCanvas.gameObject.SetActive(false);
            }
            else
            {
                mapCanvas.SetActive(true);
                mapCam.gameObject.SetActive(true);
            }
        }
        //opens main menu or closes any active menu
        if(Input.GetKeyDown("escape"))
        {
            bool openMainMenu = true;//don't need to save this value in global since escape button wont be used that often

            //if main menu is active close main menu
            if (mainMenu.activeSelf)
            {
                mainMenu.SetActive(false);
                openMainMenu = false;
            }
            else
            {
                for (int i = otherMenus.Length - 1; i >= 0; i--)
                {
                    if(otherMenus[i].activeSelf)
                    {
                        otherMenus[i].SetActive(false);
                        openMainMenu = false;
                    }
                }
            }

            if (openMainMenu) mainMenu.SetActive(true);
        }

        if(Input.GetKeyDown("1") && !items.IsNull(1))//change to 1st item
        {
            items.lastHeld = items.currentlyHeld;
            items.currentlyHeld = 1;
            shootScript.SetGunCharacteristics(items.GetHeldGunCharacteristics(),items.GetHeldGunTag());
            items.EnableSelectGunIndicator();
        }
        if (Input.GetKeyDown("2") && !items.IsNull(2))//change to 2nd item
        {
            items.lastHeld = items.currentlyHeld;
            items.currentlyHeld = 2;
            shootScript.SetGunCharacteristics(items.GetHeldGunCharacteristics(), items.GetHeldGunTag());
            items.EnableSelectGunIndicator();
        }
        if (Input.GetKeyDown("3") && !items.IsNull(3))//change to 3rd item
        {
            items.lastHeld = items.currentlyHeld;
            items.currentlyHeld = 3;
            shootScript.SetGunCharacteristics(items.GetHeldGunCharacteristics(), items.GetHeldGunTag());
            items.EnableSelectGunIndicator();
        }
        if (Input.GetKeyDown("4") && !items.IsNull(4))//change to 4th item
        {
            items.lastHeld = items.currentlyHeld;
            items.currentlyHeld = 4;
            shootScript.SetGunCharacteristics(new float[] { 0,0,0}, items.GetHeldGunTag());
            items.EnableSelectGunIndicator();
        }

        

    }

    public void SetItemOnMouse(GameObject item)
    {
        Debug.Log("SetItemOnMouse");
        isMouseOnPickable = true;
        itemOnMouse = item;
    }
    public void ClearItemOnMouse()
    {
        isMouseOnPickable = false;
        itemOnMouse = null;
    }

    // TODO MODULARITY VIOLATION LATER:: I need to do this here because i dont own the pickup object but this can be fixed easly
    [Command]
    void CmdPickedUp(GameObject pickUp)
    {
        pickUp.GetComponent<PickUp>().RpcPickedUp();
    }
}
