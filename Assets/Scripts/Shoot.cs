using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Shoot : NetworkBehaviour {


    [SerializeField] GameObject bulletPrefab;
    Transform firePoint;
    Transform firePoint1;//used for shotgun fire 
    Transform firePoint2;//used for shotgun fire 
    [SerializeField] float bulletSpeed=6f;
    [SerializeField] float bulletDestroyTime=2f;
    [SerializeField] PlayerStats playerStats;

    
    float fireRate, damage, magCapacity;
    
    string heldItemType="gun_Hand";//helps with determining which type of sound we should play etc.

    int ammoInMag;
    float timeToFire = 0f;

    [SerializeField] SkillManagerScript skillManager;
    [SerializeField] TextMeshProUGUI ammoGUI;
    [SerializeField] AudioManager audioManager;

    [SerializeField] GameObject[] gunGFXs;//in order(alphabetical) ARGFX, handGFX, pistolGFX, shotgunGFX, SMGGFX, SRGFX ///holds GFX and firepoints

    Quaternion bulletRotation;//gets firepoint + random value to make shooting a little bit more realistic/random

    private void Start()
    {
        firePoint = this.transform;//set to a random ass place to prevent getting errors when player starts with no guns and shoots
    }

    //change the gun characters in this script when currentlyHeld item changes
    public void Fire()
    {
        //do CmdFire() when it fits fireRate etc;
        if(fireRate<=2)//single fire
        {
            //single fire
            if(Time.time>timeToFire && ammoInMag>0)
            {
                skillManager.attacked = true;
                ammoInMag--;
                timeToFire = Time.time +1/(fireRate);
                if (heldItemType=="gun_Shotgun")//this is a shotgun
                {
                    bulletRotation = Quaternion.Euler(0, 0,
                        firePoint.rotation.eulerAngles.z + UnityEngine.Random.Range(-0.5f, 0.5f));
                    CmdShotgunFire(bulletRotation);
                    CmdPlay_Sound("Shotgun");
                }
                else
                {
                    //play sound
                    if (heldItemType == "gun_AR")
                    {
                        CmdPlay_Sound("AR");
                        bulletRotation = Quaternion.Euler(0, 0,
                        firePoint.rotation.eulerAngles.z + UnityEngine.Random.Range(-2.5f, 2.5f));
                    }
                    else if (heldItemType == "gun_SMG")
                    {
                        CmdPlay_Sound("SMG");
                        bulletRotation = Quaternion.Euler(0, 0,
                        firePoint.rotation.eulerAngles.z + UnityEngine.Random.Range(-5f, 5f));
                    }
                    else if (heldItemType == "gun_SR")
                    {
                        CmdPlay_Sound("SR");
                        bulletRotation = firePoint.rotation;
                    }
                    else if (heldItemType == "gun_Pistol")
                    {
                        CmdPlay_Sound("Pistol");
                        bulletRotation = Quaternion.Euler(0, 0,
                        firePoint.rotation.eulerAngles.z + UnityEngine.Random.Range(-1f, 1f));
                    }

                    CmdFire(bulletRotation);
                }


                Debug.Log("single fire initiated");
            }
        }
        else//automatic fire
        {
            if(Time.time>timeToFire && ammoInMag>0)
            {
                skillManager.attacked = true;
                ammoInMag--;
                timeToFire = Time.time + 1 / fireRate;
                Debug.Log("automatic fire initiated");

                if (heldItemType == "gun_AR")
                {
                    CmdPlay_Sound("AR");
                    bulletRotation = Quaternion.Euler(0, 0,
                        firePoint.rotation.eulerAngles.z + UnityEngine.Random.Range(-2.5f, 2.5f));
                }
                else if (heldItemType == "gun_SMG")
                {
                    CmdPlay_Sound("SMG");
                    bulletRotation = Quaternion.Euler(0, 0,
                        firePoint.rotation.eulerAngles.z + UnityEngine.Random.Range(-5f, 5f));
                }
                else if (heldItemType == "gun_SR")
                {
                    CmdPlay_Sound("SR");
                    bulletRotation = firePoint.rotation;
                }

                CmdFire(bulletRotation);
                //play sound
                
            }
        }
        ammoGUI.text = "mag \n"+ammoInMag.ToString();
    }

    [Command]
    void CmdPlay_Sound(string name)
    {
        RpcPlay_Sound(name);
    }

    [ClientRpc]
    void RpcPlay_Sound(string name)
    {
        if(name=="Shotgun")
        {
            audioManager.Play("Shotgun");
            audioManager.Play("Shotgun");
            audioManager.Play("Shotgun");
        }
        else
        {
            audioManager.Play(name);
        }
    }

    /// <summary>
    /// spawns bullets on server(and on clients)
    /// </summary>
    /// <param name="rotation">rotation of the bullets</param>
    [Command]
    void CmdFire(Quaternion rotation)
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(bulletPrefab, firePoint.position,rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * bulletSpeed * 2;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, bulletDestroyTime);
    }

    [Command]
    void CmdShotgunFire(Quaternion rotation)
    {
        // Create the Bullets from the Bullet Prefab
        var bullet = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        var bullet1 = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint1.rotation);
        var bullet2 = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint2.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * bulletSpeed;
        bullet1.GetComponent<Rigidbody2D>().velocity = bullet1.transform.right * bulletSpeed;
        bullet2.GetComponent<Rigidbody2D>().velocity = bullet2.transform.right * bulletSpeed;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);
        NetworkServer.Spawn(bullet1);
        NetworkServer.Spawn(bullet2);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, bulletDestroyTime);
        Destroy(bullet1, bulletDestroyTime);
        Destroy(bullet2, bulletDestroyTime);
    }

    public void SetGunCharacteristics(float[] characteristics,string itemType)//in order: fire rate, damage, magCapacity
    {
        Debug.Log("setting gun characteristics damage is:" + characteristics[1]+
            "magCap is:"+characteristics[2]);
        //damage is zero here
        fireRate = characteristics[0];
        damage = characteristics[1];
        magCapacity = characteristics[2];
        ammoInMag =(int) magCapacity;
        ammoGUI.text = "mag \n" + ammoInMag.ToString();

        //set new fire point & enable appropriate gfx of player on all clients
        CmdNewFirePointSetter(HeldItemTypeToGFXIndex(heldItemType), HeldItemTypeToGFXIndex(itemType));

        heldItemType = itemType;//save new item type
        
    }

    [Command]
    void CmdNewFirePointSetter(int oldGFXIndex, int newGFXIndex)
    {
        RpcNewFirePointSetter(oldGFXIndex, newGFXIndex);
    }
    [ClientRpc]
    void RpcNewFirePointSetter(int oldGFXIndex, int newGFXIndex)
    {
        gunGFXs[oldGFXIndex].SetActive(false);//disable gun GFX & firepoints etc. of old held item
        gunGFXs[newGFXIndex].SetActive(true);//enable GFX of guns & set firepoints etc. depending on new type
        //re-set firepoints(they reside in GFX gameobject)
        firePoint = gunGFXs[newGFXIndex].transform.Find("firePoint");
        firePoint1 = gunGFXs[newGFXIndex].transform.Find("firePoint1");
        firePoint2 = gunGFXs[newGFXIndex].transform.Find("firePoint2");
    }

    //TODO IENUMERATOR && individual reloads
    public void Reload()
    {
        ammoInMag =(int) magCapacity;
        ammoGUI.text = "mag \n" + ammoInMag.ToString();
    }

    int HeldItemTypeToGFXIndex(string itemType)
    {
        if (itemType == "gun_AR") return 0;
        else if (itemType == "gun_Hand") return 1;
        else if (itemType == "gun_Pistol") return 2;
        else if (itemType == "gun_Shotgun") return 3;
        else if (itemType == "gun_SMG") return 4;
        else if (itemType == "gun_SR") return 5;
        else return 1;
    }

    public Transform GetFirePoint()
    {
        return firePoint;
    }
    
    // TODO WARNING : DO NOT EVER USE LOCAL VARIABLES IN CMD
    //used to send damage to enemy with Cmd and so on
    [Command]//IMPORTANT LESSON: you cant feed local(or even syncVar) values/variables to a command function
    public void CmdSendDamage(GameObject enemy, float dmg, int armorPiercing)
    {
        enemy.GetComponent<PlayerStats>().RpcApplyDamage(dmg,this.gameObject, armorPiercing);
    }


}
