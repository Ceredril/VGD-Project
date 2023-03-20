using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cooldown
{
    public float _maxCooldown;
    public float _startCooldown;
    public float _nextCooldownTime;

    public Cooldown(float mC, float sC, float nCT) : this()
    {
        _maxCooldown = mC;
        _startCooldown = sC;
        _nextCooldownTime = nCT;
    }
}

public class AbilityManager : MonoBehaviour
{
    Cooldown Interact = new(2, 0, 0);
    Cooldown Fire = new(5, 0, 0);
    Cooldown Test = new(10, 0, 0);
    Cooldown temp;

    public static Dictionary<KeyCode, Cooldown> PlayerAbilities = new Dictionary<KeyCode, Cooldown>();

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnAbilityButtonPressed += ExecuteAbility;
        PlayerAbilities.Add(KeyCode.E, Interact);
        PlayerAbilities.Add(KeyCode.Q, Fire);
        PlayerAbilities.Add(KeyCode.F, Test);
        Debug.Log("Added Keys");
    }

    private void ExecuteAbility(KeyCode keyBind)
    {
        // Check if the Cooldown is currently on Cooldown
        if (PlayerAbilities[keyBind]._nextCooldownTime < Time.time)
        {
            // execute Cooldown
            temp = PlayerAbilities[keyBind];
            temp._nextCooldownTime = Time.time + temp._maxCooldown;
            PlayerAbilities[keyBind] = temp;
        }
    }

}
