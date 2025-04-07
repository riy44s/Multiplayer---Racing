using UnityEngine;
using Fusion;
using Random = UnityEngine.Random;

public class ItemBox : NetworkBehaviour, ICollidable {
    
    public GameObject model;
    public ParticleSystem breakParticle;
    public float cooldown = 5f;
    public Transform visuals;

    [Networked] public KartEntity Kart { get; set; }
    [Networked] public TickTimer DisabledTimer { get; set; }
    
    private ChangeDetector _changeDetector;

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(Kart):
                    OnKartChanged(this);
                    break;
            }
        }
    }

    public bool Collide(KartEntity kart) {
        if ( kart != null && DisabledTimer.ExpiredOrNotRunning(Runner) ) {
            Kart = kart;
            DisabledTimer = TickTimer.CreateFromSeconds(Runner, cooldown);
        }

        return true;
    }

    private static void OnKartChanged(ItemBox changed) { changed.OnKartChanged(); }
    private void OnKartChanged() {
        
        visuals.gameObject.SetActive(Kart == null);

        if ( Kart == null )
            return;

        breakParticle.Play();
    }

    public override void FixedUpdateNetwork() {
        base.FixedUpdateNetwork();
        
        if (DisabledTimer.ExpiredOrNotRunning(Runner) && Kart != null) {
            Kart = null;
        }
    }

}