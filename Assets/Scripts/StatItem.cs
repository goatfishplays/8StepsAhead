using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatItem : Item
{
    [Header("Stats")]

    public float health = 0;
    public float hunger = 0;
    public float temperature = 0;
    public float sadness = 0;
    public float anger = 0;
    public float fear = 0;

    public override void Use(Vector2 target)
    {
        if (useCooldownCur > 0)
        {
            return;
        }
        base.Use(target);
        Player player = owner.GetComponent<Player>();
        player.ChangeHealth(health, false);
        player.ChangeHunger(hunger);
        player.ChangeTemperature(temperature);
        player.ChangeSadness(sadness);
        player.ChangeFear(fear);
        player.ChangeAnger(anger);
    }
}
