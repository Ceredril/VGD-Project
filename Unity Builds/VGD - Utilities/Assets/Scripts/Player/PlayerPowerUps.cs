using System;
using System.Collections;
using UnityEngine;

public class PlayerPowerUps : MonoBehaviour
{
    //Cooldown parameters
    private readonly float _godModeDuration = 5;
    private readonly float _speedHackDuration = 15;
    //private readonly float _fireFistsDuration = 10;

    private readonly float _speedHackOffset = 2;

    public static bool GodModeEnabled;

    private IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
    }
    private void GodMode(float cooldown)
    {
        GodModeEnabled = true;
        Wait(cooldown);
        GodModeEnabled = false;
    }

    private void SpeedHack()
    {
        PlayerMovement.walkingSpeed *= _speedHackOffset;
        PlayerMovement.sprintSpeed *= _speedHackOffset;
        //Stop consuming stamina
        Wait(_speedHackDuration);
        PlayerMovement.walkingSpeed /= _speedHackOffset;
        PlayerMovement.sprintSpeed /= _speedHackOffset;
    }

    private void FireFist()
    {
        //TBD
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("godModePowerp")) GodMode(_godModeDuration);
        if (other.CompareTag("speedHackPowerup")) SpeedHack();
        if (other.CompareTag("fireFistsPowerup")) FireFist();
    }
}
