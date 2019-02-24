using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Melee : NetworkBehaviour {

    [SerializeField] SkillManagerScript skillManager;

    //[SerializeField]
    
    [HideInInspector] public float meleeDamage = 30f;

    [HideInInspector] public bool isMeleeAttacking = false;//used for getting Fire2 input in InputManager.update and firing at fixedUpdate
    

    [SerializeField] GameObject playerSword;

    [SerializeField] AudioManager audioManager;
    [Range(0, 1)] int randomSlashingSound;

    bool inMeleeAttack;//prevents multiple melee attacks at once

    Collider2D[] enemyColliders;//the enemies we hit with our sword


    private void FixedUpdate()
    {
        //if we got Fire2 input attack
        if(isMeleeAttacking && !inMeleeAttack)
        {
            skillManager.attacked = true;

            playerSword.SetActive(true);//enable the sword(animation plays automatically)

            inMeleeAttack = true;

            //play sound
            randomSlashingSound = UnityEngine.Random.Range(0, 2);//get an integer 0,1
            audioManager.CmdPlay("Slashing" + randomSlashingSound);
        }
        if(inMeleeAttack)
        {
            inMeleeAttack = false;
            CmdPlayerMeleeAttack();
        }
    }
    //Is called on all clients
    [ClientRpc]
    void RpcDoMeleeEffect()
    {
        playerSword.SetActive(true);
    }

    //Is called on the server
    [Command]
    void CmdPlayerMeleeAttack()
    {
        RpcDoMeleeEffect();


        //##################### ALTERNATIVE ####################### //
        ////rotate overtime
        //if (Time.fixedTime <= timeToRotate)
        //{
        //    playerSword.transform.rotation = Quaternion.Lerp(meleePoints[0].rotation,
        //        meleePoints[1].rotation, ((Time.fixedTime-startTime)/0.1f));

        //    Debug.Log( ((Time.fixedTime - startTime) / 0.1f));
        //}
        //if(Time.fixedTime>timeToRotate)
        //{
        //    Debug.Log("melee ended");
        //    inMeleeAttack = false;
        //    playerSword.SetActive(false);//disable the sword
        //}


        //make sure the sword is at the final position
        //playerSword.transform.rotation = meleePoints[1].rotation;
        //disable sword



        //##################### ALTERNATIVE ####################### //

        //if (Time.fixedTime > nextAttack) // only repeat attack after meleeAttackInterval
        //{
        //    // exTODO: disable these shit
        //    Debug.Log("Melee Attack done");

        //    nextAttack = Time.fixedTime + meleeAttackInterval;

        //    // get all colliders whose bounds touch the circle
        //    Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(transform.position,
        //        meleeRange, enemyLayer.value);//enemies are at layer 8

        //    foreach (Collider2D hit in enemyColliders)
        //    {
        //        if (hit == this.gameObject.GetComponent<Collider2D>()) continue;//if we hit ourself in melee skip
        //        Debug.Log("hit");
        //        if (hit)
        //        {
        //            //get enemies distance
        //            float enemyDistance = Vector2.Distance(hit.transform.position,
        //                transform.position);

        //            //attack if the enemy is close to you
        //            if (enemyDistance <= meleeRange)
        //            {
        //                //apply damage to the object
        //                // exTODO:  make AplyDamage function in enemy script
        //                hit.SendMessage("ApplyDamage", meleeDamage);
        //            }
        //        }
        //        else Debug.LogError("there is no hit in Collider2D[] enemyColliders");
        //    }
        //}
    }
    public float GetMeleeDamage()
    {
        return meleeDamage;
    }
    public void SetMeleeDamage(float damage)
    {
        meleeDamage = damage;
    }
}

