using System;
using System.Collections;
using UnityEngine;

public class PlayerPowerUps : MonoBehaviour
{
    //Cooldown parameters
    private readonly float _godModeDuration = 7;
    private readonly float _speedHackDuration = 15;
    private readonly float _fireFistsDuration = 10;

    private readonly float _speedHackOffset = 2;
    private readonly int _fireFistOffset = 2;
    private readonly int _fireBallMana = PlayerAttack.fireBallManaUse;

    public static bool GodModeEnabled = false;

    private IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
    }
    private void GodMode(float cooldown)
    {
        GodModeEnabled = true;
        StartCoroutine(WaitForGodMode(_godModeDuration));
    }

    private void SpeedHack()
    {
        PlayerMovement.walkingSpeed *= _speedHackOffset;
        PlayerMovement.sprintSpeed *= _speedHackOffset;
        //Stop consuming stamina
        PlayerMovement.normalMode = false;
        StartCoroutine(WaitForSpeedHAck(_speedHackDuration));
    }

    private void FireFist()
    {
        PlayerAttack._minFistDamage *= _fireFistOffset;
        PlayerAttack._maxFistDamage *= _fireFistOffset;
        PlayerAttack._fistCooldown /= 2;
        PlayerAttack._fireballCooldown /= 2;
        PlayerAttack._hasFireFist = true;
        PlayerAttack.fireBallManaUse -= _fireBallMana;
        PlayerBullet._minDamage *= 2;
        PlayerBullet._maxDamage *= 2;
        PlayerAttack._bulletSpeed *= 2;
        StartCoroutine(WaitForFireFist(_fireFistsDuration));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("godModeCollectible")) GodMode(_godModeDuration);
        if (other.CompareTag("speedHackCollectible")) SpeedHack();
        if (other.CompareTag("fireFistCollectible")) FireFist();
    }


    private IEnumerator WaitForFireFist(float time)
    {
        yield return new WaitForSeconds(time);
        PlayerAttack._hasFireFist = false;
        PlayerAttack._fistCooldown *= 2;
        PlayerAttack._minFistDamage /= _fireFistOffset;
        PlayerAttack._maxFistDamage /= _fireFistOffset;
        PlayerAttack._fireballCooldown *= 2;
        PlayerAttack.fireBallManaUse += _fireBallMana;
        PlayerBullet._minDamage /= 2;
        PlayerBullet._maxDamage /= 2;
        PlayerAttack._bulletSpeed /= 2;
    }

    private IEnumerator WaitForSpeedHAck(float time)
    {
        yield return new WaitForSeconds(time);
        PlayerMovement.normalMode = true;
        PlayerMovement.walkingSpeed /= _speedHackOffset;
        PlayerMovement.sprintSpeed /= _speedHackOffset;

    }

    private IEnumerator WaitForGodMode(float time)
    {
        yield return new WaitForSeconds(time);
        GodModeEnabled = false;
    }


}
