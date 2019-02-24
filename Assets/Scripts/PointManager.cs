using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointManager : MonoBehaviour {

    [SerializeField] SkillManagerScript skillManager;

    [SerializeField] TextMeshProUGUI[] points;


    public RectTransform[] skillsCanvas;

    [SerializeField] PlayerStats playerStats;

    int ChangeValue(int quirk, bool increase)
    {
        if (increase && skillManager.remainingPoints > 0)//if we want to increase the value
        {
            Debug.Log("increasing from: " + quirk);
            skillManager.remainingPoints--;
            //Debug.Log("quirkArmorPiercing value: " + skillManager.quirkArmorPiercing);
            skillManager.unspentPointsUGUI.text = "unspent skill \npoints: " + skillManager.remainingPoints;
            return quirk + 1;
            
        }
        else if(!increase && quirk>0)//decrease the value if we have more then 0 points in it
        {
            Debug.Log("decreasing from: " + quirk);
            skillManager.remainingPoints++;
            skillManager.unspentPointsUGUI.text = "unspent skill \npoints: " + skillManager.remainingPoints;

            return quirk - 1;
        }
        return quirk;
    }

    void ChangeValueMartialArt(bool increase)
    {
        if (!increase && skillManager.skillMartialArt > 0)//decrease the value if we have more then 0 points in it
        {
            skillManager.remainingPoints += 2;
            skillManager.skillMartialArt--;

            skillManager.martialArtCharges.text = "martial art \ncharges: " + skillManager.skillMartialArt;
            skillManager.unspentPointsUGUI.text = "unspent skill \npoints: " + skillManager.remainingPoints;
        }
    }

	public void QuirkArmorPiercing(bool increase)
    {
        skillManager.quirkArmorPiercing = 
            ChangeValue(skillManager.quirkArmorPiercing, increase);
        points[0].text = skillManager.quirkArmorPiercing.ToString();
    }

    public void QuirkMoreArmor(bool increase)
    {
        if(skillManager.remainingPoints>0 && increase)
        {
            playerStats.CmdSetArmor(playerStats.GetArmor() * 1.1f);
            playerStats.CmdSetMaxArmor(playerStats.GetMaxArmor() * 1.1f);
        }
        else if(skillManager.quirkMoreArmor>0 &&  !increase)
        {
            playerStats.CmdSetArmor(playerStats.GetArmor() * 0.9f);
            playerStats.CmdSetMaxArmor(playerStats.GetMaxArmor() * 0.9f);
        }
        skillManager.quirkMoreArmor =
            ChangeValue(skillManager.quirkMoreArmor, increase);

        

        points[1].text = skillManager.quirkMoreArmor.ToString();
    }

    public void QuirkFasterMovement(bool increase)
    {
        skillManager.quirkFastMovement =
            ChangeValue(skillManager.quirkFastMovement, increase);
        points[2].text = skillManager.quirkFastMovement.ToString();
    }

    public void QuirkMoreHealth(bool increase)
    {
        if(skillManager.remainingPoints>0 && increase)
        {
            playerStats.CmdSetHealth(playerStats.GetHealth() * 1.1f);
            playerStats.CmdSetMaxHealth(playerStats.GetMaxHealth() * 1.1f);
        }
        else if(skillManager.quirkMoreHealth>0 && !increase)
        {
            playerStats.CmdSetHealth(playerStats.GetHealth() * 0.9f);
            playerStats.CmdSetMaxHealth(playerStats.GetMaxHealth() * 0.9f);
        }
        skillManager.quirkMoreHealth = 
            ChangeValue(skillManager.quirkMoreHealth, increase);

        

        points[3].text = skillManager.quirkMoreHealth.ToString();
    }

    public void SkillInvisibility(bool increase)
    {
        skillManager.skillInvisibility = 
            ChangeValue(skillManager.skillInvisibility, increase);
        points[4].text = skillManager.skillInvisibility.ToString();
        skillsCanvas[0].gameObject.SetActive(true);
    }

    public void SkillLeap(bool increase)
    {
        skillManager.skillLeap = 
            ChangeValue(skillManager.skillLeap, increase);
        points[5].text = skillManager.skillLeap.ToString();
        skillsCanvas[1].gameObject.SetActive(true);
    }


    //martial arts give back 2 points on refund, costs scrolls, players have 1 point in it at the start
    public void SkillMartialArt(bool increase)
    {
        ChangeValueMartialArt(increase);
        points[6].text = skillManager.skillMartialArt.ToString();
        if(skillManager.skillMartialArt>0) skillsCanvas[2].gameObject.SetActive(true);
        else skillsCanvas[2].gameObject.SetActive(false);
    }
}
