using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;


public class SkillManagerScript : NetworkBehaviour {

    [SerializeField] GameObject player;// the player
    Color playerColor;// players original color
    [SerializeField] Camera playerCam;// the camera attached to the player
    [SerializeField] Shoot shootScript;

    //[HideInInspector]
    public int skillInvisibility, skillLeap, skillMartialArt;//Scrolls give martial art points(can be converted to 2 points)
    //[HideInInspector]
    public int quirkFastMovement, quirkMoreArmor, quirkMoreHealth, quirkArmorPiercing;
    //[HideInInspector]
    public int remainingPoints;
    float skillInvisibilityCooldown;
    float skillLeapCooldown;

    [SerializeField] Melee meleescript;


    [HideInInspector] public bool attacked;

    public TextMeshProUGUI unspentPointsUGUI;
    public TextMeshProUGUI martialArtCharges;

    [SerializeField] Image[] cooldownTimers;

    [SerializeField] AudioManager audioManager;
    

    private void Start()
    {
        skillMartialArt = 1;//this is used for martial art charges
        remainingPoints = 10;//used for skills and quirks
        skillInvisibilityCooldown = 0f;
        skillLeapCooldown = 0f;
        playerColor = player.GetComponent<SpriteRenderer>().color;

        //playerPrefs //TODO: reset player prefs
    }

    private void Update()
    {
        if(attacked) //TODO LATER: if we attack stop the appropriate skills etc.
        {
            StartCoroutine(SkillInvisibility(true));
            attacked = false;
        }
    }

    private void FixedUpdate()
    {
        
        //Debug.Log(player.transform.rotation);
    }
    
    float SkillValueToConstant(int skillValue)
    {
        return (float)((skillValue < 4) ? 10 * Mathf.Atan(skillValue - 4) / Mathf.PI + 5 : 11.5 * Mathf.Atan(skillValue - 4) / (Mathf.PI * 2) + 5);
    }

    /// <summary>
    /// starts invisibility skill
    /// </summary>
    /// <param name="attack">if true stops invisibility</param>
    /// <returns></returns>
    public IEnumerator SkillInvisibility(bool attack)
    {
        if(attack)//if we attacked we are not invisible anymore
        {
            player.GetComponent<SpriteRenderer>().color = playerColor;
            CmdSkillInvisibility(playerColor);
            yield return null;
        }
        else
        {
            //if skill is in cooldown
            if (Time.time <=skillInvisibilityCooldown) Debug.Log("Invisibility skill is in cooldown, " +
                 "time remaining: " + (skillInvisibilityCooldown - Time.time));
            else
            {
                skillInvisibilityCooldown = Time.time + 5f;

                Color tempColor = playerColor;
                tempColor.a = 0.3f;
                player.GetComponent<SpriteRenderer>().color = tempColor;
                //on clients
                tempColor.a = 0f;
                CmdSkillInvisibility(tempColor);

                //play invisibility sound
                audioManager.CmdPlay("SkillInvisibility");

                //start cooldownImage
                StartCoroutine(StartCooldown(0, 5f));

                //player is invisible for 0.5*e^SkillValueToConstant seconds
                yield return new WaitForSeconds(0.5f * SkillValueToConstant(skillInvisibility));
                player.GetComponent<SpriteRenderer>().color = playerColor;
                // on clients
                CmdSkillInvisibility(playerColor);
            }
        }
        
    }
    [Command]
    void CmdSkillInvisibility(Color someColor)
    {
        RpcSkillInvisibility(someColor);
    }
    [ClientRpc]
    void RpcSkillInvisibility(Color someColor)
    {
        if (isLocalPlayer) return;
        player.GetComponent<SpriteRenderer>().color = someColor;
    }
    
    public void SkillLeap()// player leaps but is not unseen, movement amount is constant
        //only cooldown time gets reduced
    {
        //if skill is in cooldown
        if (Time.time <= skillLeapCooldown) Debug.Log("Leap skill is in cooldown, " +
             "time remaining: " + (skillLeapCooldown - Time.time));
        else
        {
            //for(int i=0;i<11;i++)
            //{
            //    Debug.Log(i + ":" + (5f-5f*SkillValueToConstant(i)/10f) + "\n");
            //}
            //Debug.Log("10:" + SkillValueToConstant(10) + "    1:" + SkillValueToConstant(1) + "     5:" + SkillValueToConstant(5));

            //play skill leap sound
            audioManager.CmdPlay("SkillLeap");

            skillLeapCooldown = Time.time + (5f-5f * SkillValueToConstant(skillLeap)/10f);
            StartCoroutine(StartCooldown(1, skillLeapCooldown));

            //get the angle vector
            Vector3 angleVector = playerCam.ScreenToWorldPoint(Input.mousePosition) - player.GetComponent<Rigidbody2D>().transform.position;
            // normalize angle vector and move the player
            angleVector.z = 0;
            if (angleVector.sqrMagnitude > 49)
            {
                angleVector.Normalize();
                angleVector *= 7;
            }
            //player.transform.position += 7 * angleVector;
            Debug.Log("player leaping " + angleVector.magnitude + "units");
            CmdLeapPlayer(player, angleVector);//leap player on network to ensure clients seeing player inside a building when player leaps inside vice versa
        }
    }
    /// <summary>
    /// leaps player on all clients--ensures synced leaps, so no client will see player outside a building when player leaps inside
    /// </summary>
    /// <param name="pl">player to leap</param>
    /// <param name="vec">rotation and magnitude of leap</param>
    [Command]
    void CmdLeapPlayer(GameObject pl, Vector3 vec)
    {
        RpcLeapPlayer(pl, vec);
    }
    [ClientRpc]
    void RpcLeapPlayer(GameObject pl, Vector3 vec)
    {
        pl.transform.position += vec;
    }

    /// <summary>
    /// currentl this skill only activates invisibility and leap skills and makes melee damage 4x, then does a melee hit
    /// </summary>
    /// <returns></returns>
    public IEnumerator SkillMartialArt()
    {
        if (skillMartialArt <= 0) yield break;

        //reset skills & cooldowns
        skillInvisibilityCooldown = 0;
        skillLeapCooldown = 0;
        StartCoroutine(SkillInvisibility(true));
        //play martial art sound
        audioManager.CmdPlay("SkillMartialArt");

        skillMartialArt--;
        martialArtCharges.text = "martial art \n charges: " + skillMartialArt;
        float meleeDamage = meleescript.GetMeleeDamage();
        StartCoroutine(SkillInvisibility(false));
        SkillLeap();
        meleescript.SetMeleeDamage(4 * meleeDamage);
        yield return new WaitForSeconds(3.2f);
        meleescript.isMeleeAttacking = true;
        yield return new WaitForEndOfFrame();
        meleescript.SetMeleeDamage(meleeDamage);
        
    }

    //###########################    HELPERS    #################################################//
    //###########################    HELPERS    #################################################//
    //###########################    HELPERS    #################################################//
    //###########################    HELPERS    #################################################//
    //###########################    HELPERS    #################################################//


    /// <summary>
    /// starts the cooldown 'animation' of the desired image 
    /// <para>index gets values [0, skill count on GUI-1] </para>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="endTime"></param>
    IEnumerator StartCooldown(int index , float cooldownTime)
    {
        float startTime = Time.time;
        float elapsedTime = 0f;

        //fill the whole image first then reduce it to 0
        cooldownTimers[index].fillAmount = 1;
        cooldownTimers[index].gameObject.SetActive(true);

        while(elapsedTime <= cooldownTime)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > cooldownTime) Debug.Log("cooldown ended");
            cooldownTimers[index].fillAmount = (cooldownTime - elapsedTime) / cooldownTime;
            yield return new WaitForEndOfFrame();
            //Debug.Log("cooling endTime and Time.time --> " +endTime+"  "+Time.time);
        }

        
        //cooldownTimers[index].gameObject.SetActive(false);
    }


    //currently used ONLY for leap skill
    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector2 end, float seconds)
    {
        
        float elapsedTime = 0;
        Vector2 startingPos = objectToMove.transform.position;
        bool objectMoved = false;
        while (elapsedTime < seconds)
        {
            //objectToMove.transform.rotation = Quaternion.Euler(0f,0f, Mathf.Atan2(objectToMove.transform.rotation.y, objectToMove.transform.rotation.x) * Mathf.Rad2Deg);
            objectToMove.transform.position = Vector2.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            if(CrossPlatformInputManager.GetAxis("Horizontal")!=0f || CrossPlatformInputManager.GetAxis("Vertical")!=0f)
            {
                objectMoved = true;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        if (!objectMoved) objectToMove.transform.position = end;
        
    }

    public IEnumerator MoveOverSpeed(GameObject objectToMove, Vector2 end, float speed)//has bugs // check the MoveOverSeconds func
    {

        // speed should be 1 unit per second
        while (objectToMove.transform.position.x != end.x || objectToMove.transform.position.y!= end.y)
        {
            objectToMove.transform.position = Vector2.MoveTowards(objectToMove.transform.position, end, speed * Time.fixedDeltaTime);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
