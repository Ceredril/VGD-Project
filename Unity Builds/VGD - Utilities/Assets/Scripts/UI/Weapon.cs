using UnityEngine.UI;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Image fist;
    public Image bullet;
    public Image shield;
    public Image border1;
    public Image border2;
    public Image border3;

    private void Awake()
    {
        border1.enabled = false;
        border2.enabled = false;
        border3.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        updateCurrentAttack();
    }

    private void updateCurrentAttack()
    {
        if (PlayerAttack._hasFist)
            fist.enabled = true;
        else
            fist.enabled = false;

        if (PlayerAttack._hasFireball == true)
            bullet.enabled = true;
        else
            bullet.enabled = false;

        if (PlayerAttack._hasShield)
            shield.enabled = true;
        else
            shield.enabled = false;


        if (PlayerAttack._currentSkill == PlayerAttack.Skill.Fist)
        {
            border1.enabled = true;
            border2.enabled = false;
            border3.enabled = false;
        }
        else if (PlayerAttack._currentSkill == PlayerAttack.Skill.Fireball)
        {
            border1.enabled = false;
            border2.enabled = true;
            border3.enabled = false;
        }
        if (PlayerAttack._currentSkill == PlayerAttack.Skill.Shield)
        {
            border1.enabled = false;
            border2.enabled = false;
            border3.enabled = true;
        }

    }

}
