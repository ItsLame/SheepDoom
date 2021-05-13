using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySkillsScript : MonoBehaviour
{
    [SerializeField] private GameObject SkillsText;
    // ----------------------- alma --------------------------
    public void AlmaS1()
    {
        Debug.Log("a1 clicked");
        SkillsText.GetComponent<Text>().text = "Alma fires fast but decelerating bullet that slows the enemy.";
    }

    public void AlmaS2()
    {
        Debug.Log("a2 clicked");
        SkillsText.GetComponent<Text>().text = "Alma plants a mine that stuns enemies on contact. The planted mine has a long duration.";
    }

    public void AlmaU1()
    {
        Debug.Log("a3 clicked");
        SkillsText.GetComponent<Text>().text = "Alma fires a piercing laser out of her supercharged rifle, dealing high damage to hit enemies. Has global range.";
    }

    public void AlmaU2()
    {
        Debug.Log("a4 clicked");
        SkillsText.GetComponent<Text>().text = "Modifying her rifle, Alma fires a wider but slower laser that accelerates over time. Deals moderate damage to hit enemies & has global range";
    }

    // ------------------------- luigi ----------------------
    public void LuigiS1()
    {
        Debug.Log("l1 clicked");
        SkillsText.GetComponent<Text>().text = "Luigi deploys a shield that blocks enemy projectile attacks. Piercing attacks can't be blocked.";
    }

    public void LuigiS2()
    {
        Debug.Log("l2 clicked");
        SkillsText.GetComponent<Text>().text = "Fueled by rage, Luigi's attack speed increases for a short duration.";
    }

    public void LuigiU1()
    {
        Debug.Log("l3 clicked");
        SkillsText.GetComponent<Text>().text = "Luigi wells up his inner fury, shocking frontal enemies with fear after a set duration. Shock status is removed after enemy takes damage.";
    }

    public void LuigiU2()
    {
        Debug.Log("l4 clicked");
        SkillsText.GetComponent<Text>().text = "Luigi goes into a frenzy, greatly increasing movement speed for a long duration.";
    }

    // ------------------------------- isa ---------------------------
    public void IsaS1()
    {
        SkillsText.GetComponent<Text>().text = "Isabelle sends out a ray of god's judgement that radiates on enemy contact, dealing damage to hit enemies and healing surrounding allies.";
    }

    public void IsaS2()
    {
        SkillsText.GetComponent<Text>().text = "Focusing holy magic into a ball, Isabelle fires a healing projectile that heals allies on contact.";
    }

    public void IsaU1()
    {
        SkillsText.GetComponent<Text>().text = "Isabelle starts incanting prayers. After a casting duration, Isabelle heals allies in a large AOE around her.";
    }

    public void IsaU2()
    {
        SkillsText.GetComponent<Text>().text = "The Goddess is both merciful and cruel. Isabelle channels a holy zone around her, " +
            "healing allies & damaging surrounding enemies over time. The channeling is stopped when Isabelle moves.";
    }
}
