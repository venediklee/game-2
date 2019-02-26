using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;


public class PlayerItems : NetworkBehaviour {
    
    //players can carry a max. of 2 guns(can be pistols), 1 pistol and 1 kit
    public GameObject gun1;
    GameObject gun2;
    GameObject pistol1;
    GameObject kit1;

    [SerializeField] Shoot shootScript;
    [SerializeField] PlayerStats stats;

    //in order: gun1, gun2, pistol1, kit1;
    [SerializeField] RectTransform[] itemsCanvas;
    [SerializeField] RectTransform[] itemSelectedIndicators;

    [SerializeField] RectTransform useKitGFX;
    [SerializeField] RectTransform errorMessageGFX;//used for showing errors like health is max, cant use healt potion etc. on screen

    [SerializeField] Image kitCooldownTimer;

    //assign values to these when you change items
    [HideInInspector] public int currentlyHeld=0, lastHeld=0;//currently held object (gun1->1 ,gun2->2 ,pistol1->3 ,kit1->4)
                                                 //used for dropping, shoot calculations etc.
    [HideInInspector] public bool attacked, moved;//used for stopping kit usage etc.

    [SerializeField] SkillManagerScript skillManager;

    [SerializeField] TextMeshProUGUI skillMenuMartialArtCharges;

    [SerializeField] PointManager pointManager;

    //returns true if we successfully pick up item
    public bool SetFirstEmptyGun(GameObject gun)//puts the gun to the first empty gun slot
        //if there is no empty gun socket change the currentlyHeld gun 
    {
        Debug.Log("setting first empty gun");

        if (gun.CompareTag("gun_Pistol"))//if the gun is pistol
        {
            if (pistol1 == null)//if pistol slot is empty
            {
                ChangeItemOnCanvas(2, gun);
                pistol1 = gun;
            }
            else if (gun1 == null)
            {
                ChangeItemOnCanvas(0, gun);
                gun1 = gun;
            }
            else if (gun2 == null)
            {
                ChangeItemOnCanvas(1, gun);
                gun2 = gun;
            }
            else//player wants to drop the currentlyHeld pistol for the new pistol
            {
                //drop pistol
                CmdDropGun(pistol1, this.transform.position);
                //set new pistol
                pistol1 = gun;
                ChangeItemOnCanvas(2, gun);
            }


            //in case this is the first gun player gets
            if (currentlyHeld == 0)
            {
                Debug.Log("that was the first pistol we picked");
                currentlyHeld = 3;
                Debug.Log("damage is" + GetHeldGunCharacteristics()[1]);
            }

        }

        else if (gun.CompareTag("gun_AR") || gun.CompareTag("gun_SMG") || 
            gun.CompareTag("gun_Shotgun") || gun.CompareTag("gun_SR")) //if the gun is gun(like a rifle etc.)
        {
            if (gun1 == null)
            {
                ChangeItemOnCanvas(0, gun);
                gun1 = gun;
            }

            else if (gun2 == null)
            {
                ChangeItemOnCanvas(1, gun);
                gun2 = gun;
            }
            else//player wants to drop the currentlyHeld weapon for the new gun
            {
                if (currentlyHeld >= 3) return false;// can't set the 3rd and 4th slots to guns
                
                if(currentlyHeld==1)//change first gun
                {
                    //drop gun
                    CmdDropGun(gun1, this.transform.position);
                    //set new gun
                    gun1 = gun;
                }
                else if(currentlyHeld==2)
                {
                    // drop gun
                    CmdDropGun(gun2, this.transform.position);
                    //set new gun
                    gun2 = gun;
                }
                ChangeItemOnCanvas(currentlyHeld - 1, gun);
            }
            //in case this is the first gun player gets
            if (currentlyHeld == 0)
            {
                Debug.Log("that was the first gun we picked");
                currentlyHeld = 1;
                Debug.Log("damage is" + GetHeldGunCharacteristics()[1]);
            }
        }

        else//player picks up kit(tag= healthKit || armorKit || scrollKit)
        {
            if (currentlyHeld == 0) currentlyHeld = 4;
            if (kit1 != null) CmdDropGun(kit1, this.transform.position);//drop the kit if we already have one
            ChangeItemOnCanvas(3, gun);
            kit1 = gun;
        }

        shootScript.SetGunCharacteristics(GetHeldGunCharacteristics(), GetHeldGunTag());//re-set held gun characteristics
        EnableSelectGunIndicator();//indicate which gun we are holding
        return true;
    }

    /// <summary>
    /// index has [0,3] values only
    /// </summary>
    /// <param name="index"></param>
    /// <param name="gun"></param>
    void ChangeItemOnCanvas(int index, GameObject gun)
    {
        if(gun==null)//putting an empty object
        {
            itemsCanvas[index].GetComponent<Image>().overrideSprite = null;
            Color tempColorNull = itemsCanvas[index].GetComponent<Image>().color;
            tempColorNull.a = 0.3921f;
            itemsCanvas[index].GetComponent<Image>().color = tempColorNull;
            return;
        }
        itemsCanvas[index].GetComponent<Image>().overrideSprite = gun.GetComponent<SpriteRenderer>().sprite;
        Color tempColor = itemsCanvas[index].GetComponent<Image>().color;
        tempColor.a = 1f;
        itemsCanvas[index].GetComponent<Image>().color = tempColor;
    }
    public void DropGun()
    {
        //do CmdDropGun(currentGun);
        ChangeItemOnCanvas(currentlyHeld - 1, null);
        switch (currentlyHeld)
        {
            case 1:
                CmdDropGun(gun1,this.gameObject.transform.position);
                itemSelectedIndicators[0].gameObject.SetActive(false);
                gun1 = null;
                break;

            case 2:
                CmdDropGun(gun2, this.gameObject.transform.position);
                itemSelectedIndicators[1].gameObject.SetActive(false);
                gun2 = null;
                break;

            case 3:
                CmdDropGun(pistol1, this.gameObject.transform.position);
                itemSelectedIndicators[2].gameObject.SetActive(false);
                pistol1 = null;
                break;

            case 4:
                CmdDropGun(kit1, this.gameObject.transform.position);
                itemSelectedIndicators[3].gameObject.SetActive(false);
                kit1 = null;
                break;

            default:
                Debug.LogError("currentlyHeld variable has wrong value");
                break;
        }
        if (gun1 != null) currentlyHeld = 1;
        else if (gun2 != null) currentlyHeld = 2;
        else if (pistol1 != null) currentlyHeld = 3;
        else if (kit1 != null) currentlyHeld = 4;
        else currentlyHeld = 0;
        shootScript.SetGunCharacteristics(GetHeldGunCharacteristics(), GetHeldGunTag());
        EnableSelectGunIndicator();
    }

    /// <summary>
    /// drops desired gun to the ground on all clients
    /// </summary>
    /// <param name="gun"></param>
    /// <param name="newPosition">X and Y coordinates of new position, set Z to -1</param>
    [Command]
    public void CmdDropGun(GameObject gun,Vector2 newPosition)
    {
        Debug.Log("CmdDropGun");
        gun.transform.position = new Vector3(newPosition.x, newPosition.y, -1);
        RpcDroppedDown(gun,newPosition);
    }
    [ClientRpc]
    public void RpcDroppedDown(GameObject gun,Vector2 newPosition)
    {
        Debug.Log("RpcDropped");
        gun.SetActive(true);
        gun.transform.position = new Vector3(newPosition.x, newPosition.y, -1);
    }

    void ChangeItem(GameObject gun)//player picks a new gun from the floor
    {
        //DropGun();// drop the currentlyHeld to ground,
            //then take a new one
        switch(currentlyHeld)
        {
            case 1:
                gun1 = gun;
                currentlyHeld = 1;
                break;

            case 2:
                gun2 = gun;
                currentlyHeld = 2;
                break;

            case 3:
                pistol1 = gun;
                currentlyHeld = 3;
                break;

            case 4:
                kit1 = gun;
                currentlyHeld = 4;
                break;

            default: Debug.LogError("currentlyHeld variable has wrong value");
                break;
        }
    }

    public IEnumerator UseKit()//tag= healthKit || armorKit
    {
        GameObject kit = kit1;
        if (kit.CompareTag("healthKit"))
        {
            if (stats.GetHealth() >= stats.GetMaxHealth())//@ max health
            {
                Debug.Log(this.gameObject.name + "is already at max health");
                StartCoroutine(DisplayErrorMessage("player is already at maximum health"));
                yield break;
            }
        }
        else if(kit1.CompareTag("armorKit"))
        {
            if (stats.GetArmor()>= stats.GetMaxArmor())//@ max armor
            {
                Debug.Log(this.gameObject.name + "is already at max armor");
                yield break;
            }
        }
        else//(kit1.CompareTag("scrollKit"))
        {
            //max skillPoints???
            //nothing for now
        }

        kit1 = null;//make the kit null to prevent second use

        //fill kit cooldown timer fully, then decrease the fill with time
        kitCooldownTimer.fillAmount = 1f;

        float startTime = Time.time;
        
        float endTime = Time.time + 2f;
        while(Time.time<endTime)
        {
            if (attacked || moved)
            {
                Debug.Log("UseKit stopped");
                kit1 = kit;
                ChangeItemOnCanvas(3, kit1);
                kitCooldownTimer.fillAmount = 0f;
                yield break;
            }
            else//we continue to apply kit
            {
                //decrease the fill with time
                kitCooldownTimer.fillAmount = (endTime - Time.time) / 2f;
                yield return new WaitForEndOfFrame();
            }
        }
        ChangeItemOnCanvas(3, null);//erase kit from canvas

        //we used the kit, now set the current gun again
        currentlyHeld = lastHeld;
        lastHeld = 0;
        EnableSelectGunIndicator();
        itemSelectedIndicators[3].gameObject.SetActive(false);//need to manually set this
        shootScript.SetGunCharacteristics(GetHeldGunCharacteristics(), GetHeldGunTag());

        if (kit.CompareTag("healthKit"))
        {
            Debug.Log("+50 hp");
            stats.CmdSetHealth(stats.GetHealth() + 50f);
        }
        else if(kit.CompareTag("armorKit"))
        {
            stats.CmdSetArmor(stats.GetArmor() + 5f);
        }
        else//(kit.CompareTag("scrollKit"))
        {
            skillManager.skillMartialArt++;
            skillManager.martialArtCharges.text = "martial art \ncharges: " + skillManager.skillMartialArt;
            skillMenuMartialArtCharges.text = skillManager.skillMartialArt.ToString();
            pointManager.skillsCanvas[2].gameObject.SetActive(true);
        }
    }

    public float[] GetHeldGunCharacteristics()//return in order: fire rate, damage, magCapacity
    {
        //character.fireRate, character.damage, character.magCapacity
        GunCharacter character = null;
        switch (currentlyHeld)
        {
            case 1:
                character = gun1.GetComponent<GunCharacter>();
                break;

            case 2:
                character = gun2.GetComponent<GunCharacter>();
                break;

            case 3:
                character = pistol1.GetComponent<GunCharacter>();
                break;

            case 4:
                Debug.LogWarning("cant shoot with kit");
                return new float[] { 0, 0, 0 };

            default:
                Debug.LogWarning("currentlyHeld variable could have the wrong value: "+currentlyHeld);
                return new float[] { 0, 0, 0 };
        }
        
        return new float[] { character.fireRate, character.damage, character.magCapacity };
    }
    /// <summary>
    /// returns tag of the currently geld item
    /// </summary>
    /// <returns></returns>
    public string GetHeldGunTag()
    {
        switch (currentlyHeld)
        {
            case 1:
                return gun1.tag;

            case 2:
                return gun2.tag;

            case 3:
                return pistol1.tag;

            case 4:
                return kit1.tag;

            default:
                return "none";
        }
    }

    /// <summary>
    /// returns true if the item @number-1 index is null
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public bool IsNull(int number)//used @input manager for changing guns etc.
    {
        switch (number)
        {
            case 1: return gun1 == null;
            case 2: return gun2 == null;
            case 3: return pistol1 == null;
            case 4: return kit1 == null;
            default: Debug.LogError("IsNull function is given wrong value");
                return true;
        }
    }

    /// <summary>
    /// enables the indicator that the currently held item is active
    /// </summary>
    public void EnableSelectGunIndicator()
    {
        if (lastHeld>0) itemSelectedIndicators[lastHeld - 1].gameObject.SetActive(false);
        if(currentlyHeld>0) itemSelectedIndicators[currentlyHeld-1].gameObject.SetActive(true);
        else
        {
            for (int i = 0; i < 4; i++) itemSelectedIndicators[i].gameObject.SetActive(false);
        }

        if (currentlyHeld == 4) useKitGFX.gameObject.SetActive(true);
        else useKitGFX.gameObject.SetActive(false);
    }

    /// <summary>
    /// displays error message on screen
    /// </summary>
    /// <param name="s">message to display</param>
    public IEnumerator DisplayErrorMessage(string errorMessage)
    {

        TextMeshPro err = errorMessageGFX.GetComponent<TextMeshPro>();
        err.gameObject.SetActive(true);
        err.text = errorMessage;

        Debug.Log("name of error message gameobject::"+err.gameObject.name);

        //display the text for 1 second with blinking(changing alpha)
        float passedTime = 0f;
        float duration = 1f;
        Color startColor = err.color;//cant impilictly change alpha of text, so change all color variable
        while(passedTime<duration)
        {
            startColor.a =1 - passedTime%0.5f;
            err.color = startColor;
            passedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        err.gameObject.SetActive(false);
    }
}
