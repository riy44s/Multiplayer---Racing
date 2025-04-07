using Fusion;
using UnityEngine;

public class KartCooldownTimer : KartComponent
{
    [Networked]
    public TickTimer CooldownTimer { get; set; }

    public float CooldownDuration { get; set; } = 3f;

    public bool IsCooldownActive => !CooldownTimer.ExpiredOrNotRunning(Runner);

    public void StartCooldown(float duration = 0f)
    {
        if (duration <= 0) duration = CooldownDuration;
        CooldownTimer = TickTimer.CreateFromSeconds(Runner, duration);
    }
}