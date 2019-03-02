using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerStats : NetworkBehaviour {

    float maxHealth,maxArmor;
    [SyncVar(hook = "OnChangeHealth")][SerializeField] float playerHealth;
    [SyncVar][SerializeField] float playerArmor;

    [SerializeField] RectTransform healthBar;

    //[HideInInspector]
    public int killCount;
    [SerializeField] SkillManagerScript skillManager;

    [SerializeField] AudioManager audioManager;

    [SerializeField] RectTransform RageGFX;
    [SerializeField] RectTransform[] RunGFXs;//8 GFX ONLY 

    // Use this for initialization
    void Start()
    {
        playerHealth = 100;
        maxHealth = playerHealth;

        playerArmor = 5f;
        maxArmor = 20f;
        
    }



    public float GetHealth()
    {
        return playerHealth;
    }

    public float GetArmor()
    {
        return playerArmor;
    }

    [Command]
    public void CmdSetHealth(float newHealth) { RpcSetHealth(newHealth); }

    [ClientRpc]
    void RpcSetHealth(float newHealth) { playerHealth = Mathf.Min(maxHealth, newHealth); }

    [Command]
    public void CmdSetMaxHealth(float newMaxHealth) { RpcSetMaxHealth(newMaxHealth); }

    [ClientRpc]
    void RpcSetMaxHealth(float newMaxHealth) { maxHealth = newMaxHealth; }

    [Command]
    public void CmdSetArmor(float newArmor) { RpcSetArmor(newArmor); }

    [ClientRpc]
    void RpcSetArmor(float newArmor) { playerArmor = Mathf.Min(maxArmor, newArmor); }

    [Command]
    public void CmdSetMaxArmor(float newMaxArmor) { RpcSetMaxArmor(newMaxArmor); }

    [ClientRpc]
    void RpcSetMaxArmor(float newMaxArmor) { maxArmor = newMaxArmor; }

    public float GetMaxHealth() { return maxHealth; }
    public float GetMaxArmor() { return maxArmor; }

    [Command]
    public void CmdApplyDamage(float damage, GameObject shooter, int armorPiercing)
    {
        RpcApplyDamage(damage, shooter, armorPiercing);
    }
    [ClientRpc]
    public void RpcApplyDamage( float damage, GameObject shooter, int armorPiercing )
    {

        playerHealth -= Mathf.Max(0, damage - Mathf.Max(playerArmor - armorPiercing, 0));       

        Debug.Log("Player Received Net Damage:" + Mathf.Max(0, damage - Mathf.Max(playerArmor - armorPiercing, 0)));
        // if our health is 0 or less
        if (playerHealth <= 0)
        {
            
            playerHealth = 0;
            //TODO BALANCING
            if(UnityEngine.Random.value*killCount>2)//player does not die but RAGEs
            {
                Debug.Log("RAGE");
                //play rage sound depending on number of kills
                //(3 kills=1.2 pitch, 4 kills 1.1 pitch... 8+ kills 0.7 pitch
                float pitch = 1.2f - (killCount - 3) / 10;//new pitch value of Rage sound
                if (pitch < 0.7f) pitch = 0.7f;
                Array.Find(audioManager.sounds, item => item.name == "Rage").pitch = pitch;//set pitch value of rage sound
                audioManager.CmdPlay("Rage");

                //activate RAGE GFX & RUN GFXs
                RageGFX.gameObject.SetActive(true);
                switch (killCount)
                {
                    //cant utilize fall dawn since I need to set angles based on kill count
                    case 3://spawn 3 run gfx
                        RunGFXs[0].gameObject.SetActive(true); RunGFXs[0].rotation = Quaternion.Euler(0, 0, 0);
                        RunGFXs[1].gameObject.SetActive(true); RunGFXs[1].rotation = Quaternion.Euler(0, 0, 120);
                        RunGFXs[2].gameObject.SetActive(true); RunGFXs[2].rotation = Quaternion.Euler(0, 0, 240);
                        break;
                    case 4://spawn 4 run gfx
                        RunGFXs[0].gameObject.SetActive(true); RunGFXs[0].rotation = Quaternion.Euler(0, 0, 0);
                        RunGFXs[1].gameObject.SetActive(true); RunGFXs[1].rotation = Quaternion.Euler(0, 0, 90);
                        RunGFXs[2].gameObject.SetActive(true); RunGFXs[2].rotation = Quaternion.Euler(0, 0, 180);
                        RunGFXs[3].gameObject.SetActive(true); RunGFXs[3].rotation = Quaternion.Euler(0, 0, 270);
                        break;
                    case 5://spawn 5 run gfx
                        RunGFXs[0].gameObject.SetActive(true); RunGFXs[0].rotation = Quaternion.Euler(0, 0, 0);
                        RunGFXs[1].gameObject.SetActive(true); RunGFXs[1].rotation = Quaternion.Euler(0, 0, 72);
                        RunGFXs[2].gameObject.SetActive(true); RunGFXs[2].rotation = Quaternion.Euler(0, 0, 144);
                        RunGFXs[3].gameObject.SetActive(true); RunGFXs[3].rotation = Quaternion.Euler(0, 0, 216);
                        RunGFXs[4].gameObject.SetActive(true); RunGFXs[4].rotation = Quaternion.Euler(0, 0, 288);
                        break;
                    case 6://spawn 6 run gfx
                        RunGFXs[0].gameObject.SetActive(true); RunGFXs[0].rotation = Quaternion.Euler(0, 0, 0);
                        RunGFXs[1].gameObject.SetActive(true); RunGFXs[1].rotation = Quaternion.Euler(0, 0, 60);
                        RunGFXs[2].gameObject.SetActive(true); RunGFXs[2].rotation = Quaternion.Euler(0, 0, 120);
                        RunGFXs[3].gameObject.SetActive(true); RunGFXs[3].rotation = Quaternion.Euler(0, 0, 180);
                        RunGFXs[4].gameObject.SetActive(true); RunGFXs[4].rotation = Quaternion.Euler(0, 0, 240);
                        RunGFXs[5].gameObject.SetActive(true); RunGFXs[5].rotation = Quaternion.Euler(0, 0, 300);
                        break;
                    case 7://spawn 7 run gfx
                        RunGFXs[0].gameObject.SetActive(true); RunGFXs[0].rotation = Quaternion.Euler(0, 0, 0);
                        RunGFXs[1].gameObject.SetActive(true); RunGFXs[1].rotation = Quaternion.Euler(0, 0, 51.4f); 
                        RunGFXs[2].gameObject.SetActive(true); RunGFXs[2].rotation = Quaternion.Euler(0, 0, 102.8f); 
                        RunGFXs[3].gameObject.SetActive(true); RunGFXs[3].rotation = Quaternion.Euler(0, 0, 154.2f); 
                        RunGFXs[4].gameObject.SetActive(true); RunGFXs[4].rotation = Quaternion.Euler(0, 0, 205.6f); 
                        RunGFXs[5].gameObject.SetActive(true); RunGFXs[5].rotation = Quaternion.Euler(0, 0, 257); 
                        RunGFXs[6].gameObject.SetActive(true); RunGFXs[6].rotation = Quaternion.Euler(0, 0, 308.4f); 
                        break;
                    default://kill count >=8 spawn 8 run gfx
                        RunGFXs[0].gameObject.SetActive(true); RunGFXs[0].rotation = Quaternion.Euler(0, 0, 0);
                        RunGFXs[1].gameObject.SetActive(true); RunGFXs[1].rotation = Quaternion.Euler(0, 0, 45);
                        RunGFXs[2].gameObject.SetActive(true); RunGFXs[2].rotation = Quaternion.Euler(0, 0, 90);
                        RunGFXs[3].gameObject.SetActive(true); RunGFXs[3].rotation = Quaternion.Euler(0, 0, 135);
                        RunGFXs[4].gameObject.SetActive(true); RunGFXs[4].rotation = Quaternion.Euler(0, 0, 180);
                        RunGFXs[5].gameObject.SetActive(true); RunGFXs[5].rotation = Quaternion.Euler(0, 0, 225);
                        RunGFXs[6].gameObject.SetActive(true); RunGFXs[6].rotation = Quaternion.Euler(0, 0, 270);
                        RunGFXs[7].gameObject.SetActive(true); RunGFXs[7].rotation = Quaternion.Euler(0, 0, 315);
                        break;
                }
                playerHealth = 10*(skillManager.quirkMoreHealth);
            }
            else//player dies
            {
                if(shooter!=null && shooter.CompareTag("RemotePlayer"))
                {
                    (shooter.GetComponent<PlayerStats>().killCount)++;
                    (shooter.GetComponent<SkillManagerScript>().remainingPoints)++;//give player skill point for killing
                }
                //first disable player, then destroy
                gameObject.GetComponent<CircleCollider2D>().enabled = false;
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<InputManagerScript>().enabled = false;
                gameObject.GetComponent<Movement>().enabled = false;
                Destroy(gameObject,1f);
                //play death sound
                audioManager.CmdPlay("Death");
                Debug.Log("Player Died");
            }
        }
    }
    
    ///// <summary>
    ///// activates Rage GFX in raged players position with kills number of RUN GFX
    ///// </summary>
    ///// <param name="kills">number of kills player has, effects number of RUN GFX spawned</param>
    ///// <param name="ragedPlayer">player who raged</param>
    ///// <returns></returns>
    //IEnumerator RageGFXActivator(int kills, GameObject ragedPlayer)
    //{
    //    var rageGfx = Instantiate(RageGFXPrefab, ragedPlayer.transform);
    //    rageGfx
    //}

    void OnChangeHealth(float playerHealth)
    {
        //change healthbar size
        healthBar.sizeDelta = new Vector2(playerHealth, healthBar.sizeDelta.y);
    }
}
