using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//TODO LATER:: show next safezone area(also move center of safezone )
public class Radiation : NetworkBehaviour {

    [SerializeField] float shrinkCooldown = 100f;
    [SerializeField] float shrinkDuration = 20f;
    [SerializeField] float shrinkFactor = 0.75f;//used for shrinking ratio & shrinking cooldown ratio & shrink duration ratio
    GameObject player;

    float radius;//radius of safe zone(circle collider)
    int stage = 0;//indicates stage of safe zone--becomes 1 at the start of execution
    CircleCollider2D safeZone;

    Coroutine radDamageCoroutine;//used for storing StartRadiationDamage coroutine

    Shapes2D.Shape safeZoneProps;//used for safeZone 'sprite' (shrinking etc.)
    float safeZoneSize;//used for calculating inner cutout of safeZone 'sprite'

    private void Start()
    {
        safeZone = this.GetComponent<CircleCollider2D>();

        safeZoneProps = this.GetComponentInChildren<Shapes2D.Shape>();
        safeZoneSize = this.transform.GetChild(0).localScale.x;

        //start stages of safe zone
        StartCoroutine(NextStage());
    }

    public override void OnStartLocalPlayer()
    {
       // player = GameObject.FindGameObjectWithTag("localPlayer");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("localPlayer"))//if player moves out of safe zone
        {
            player = collision.gameObject;
            radDamageCoroutine = StartCoroutine(StartRadiationDamage());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("localPlayer"))
        {
            //stop radiation damage
            if(radDamageCoroutine!=null) StopCoroutine(radDamageCoroutine);
        }
    }
    IEnumerator StartRadiationDamage()//damages player every 5 seconds while he is outside the safezone, stop this when the player is inside the safezone
    {
        //wait 5 seconds
        yield return new WaitForSecondsRealtime(5f);

        //check players distance to the center of safe zone;
        //if distance is smaller than radius of safe zone dont deal damage, else deal damage depending on the stage of safe zone
        if (player!=null && (player.transform.position - this.transform.position).magnitude > safeZone.radius)
        {
            player.GetComponent<PlayerStats>().RpcApplyDamage(5 * stage, null, (int)player.GetComponent<PlayerStats>().GetArmor());//radiaton bypasses armor
            //restart the radiation damage
            radDamageCoroutine = StartCoroutine(StartRadiationDamage());
        }        
    }

    IEnumerator NextStage()//used for calling ShrinkSafeZone function every X seconds
    {
        stage++;

        Debug.Log("stage" + stage + " starts in" + shrinkCooldown + ", duration:" + shrinkDuration+",   timestep:"+Time.time);
        
        yield return new WaitForSecondsRealtime(shrinkCooldown);

        StartCoroutine(ShrinkSafeZone());
        shrinkCooldown *= shrinkFactor;
        shrinkDuration *= shrinkFactor;
    }

    IEnumerator ShrinkSafeZone()
    {
        float startTime = Time.time;
        float passedTime = Time.time - startTime;
        float startRadius = safeZone.radius;
        float targetRadius = startRadius * shrinkFactor;

        float innerCutOutSize;//inner cut out size of safezone 'sprite'

        while (shrinkDuration>=passedTime)
        {
            safeZone.radius = startRadius - (startRadius - targetRadius) * passedTime / shrinkDuration;//setting radius to a value between startRadius & targetRadius,proportional to passed time(lerp)
            innerCutOutSize = (safeZone.radius * 2) / safeZoneSize;
            
            safeZoneProps.settings.innerCutout = new Vector2(innerCutOutSize,innerCutOutSize);//setting the safezone sprite

            yield return new WaitForEndOfFrame();//this ensures a continuous shrink
            passedTime = Time.time - startTime;
        }
        Debug.Log("shrinking ended, timestep:" + Time.time);
        //at this point safeZone radius is shrinked enough.
        //call nextStage--handles shrink cooldown etc.
        StartCoroutine(NextStage());
        
    }
    
}
