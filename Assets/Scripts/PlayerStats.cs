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
    public float killCount;
    [SerializeField] SkillManagerScript skillManager;

    [SerializeField] AudioManager audioManager;

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

    void OnChangeHealth(float playerHealth)
    {
        //change healthbar size
        healthBar.sizeDelta = new Vector2(playerHealth, healthBar.sizeDelta.y);
    }
}
