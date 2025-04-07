using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public interface IGameUIComponent
    {
        void Init(KartEntity entity);
    }

    public CanvasGroup fader;
    public Animator countdownAnimator;
    public GameObject timesContainer;
    public GameObject coinCountContainer;
    public GameObject lapCountContainer;
    public EndRaceUI endRaceScreen;
    public Image boostBar;
    public Text coinCount;
    public Text lapCount;
    public Text raceTimeText;
    public Text[] lapTimeTexts;
    public Text introGameModeText;
    public Text introTrackNameText;
    public Button continueEndButton;
    private bool _startedCountdown;

    public KartEntity Kart { get; private set; }
    private KartController KartController => Kart.Controller;

    public void Init(KartEntity kart)
    {
        Kart = kart;

        var uis = GetComponentsInChildren<IGameUIComponent>(true);
        foreach (var ui in uis) ui.Init(kart);


        var track = Track.Current;

        if (track == null)
            Debug.LogWarning($"You need to initialize the GameUI on a track for track-specific values to be updated!");
        else
        {
            introGameModeText.text = GameManager.Instance.GameType.modeName;
            introTrackNameText.text = track.definition.trackName;
        }

        GameType gameType = GameManager.Instance.GameType;

        if (gameType.IsPracticeMode())
        {
            timesContainer.SetActive(false);
            lapCountContainer.SetActive(false);
        }

        if (gameType.hasCoins == false)
        {
            coinCountContainer.SetActive(false);
        }

        continueEndButton.gameObject.SetActive(kart.Object.HasStateAuthority);

        kart.OnCoinCountChanged += count =>
        {
            coinCount.text = $"{count:00}";
        };
    }


    public void FinishCountdown()
    {
        // Kart.OnRaceStart();
    }

    private void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float t = 1;
        while (t > 0)
        {
            fader.alpha = 1 - t;
            t -= Time.deltaTime;
            yield return null;
        }
    }

    private void Update()
    {
        if (!Kart || !Kart.LapController.Object || !Kart.LapController.Object.IsValid)
            return;

        if (!_startedCountdown && Track.Current != null && Track.Current.StartRaceTimer.IsRunning)
        {
            var remainingTime = Track.Current.StartRaceTimer.RemainingTime(Kart.Runner);
            if (remainingTime != null && remainingTime <= 3.0f)
            {
                _startedCountdown = true;
                FadeIn();
                countdownAnimator.SetTrigger("StartCountdown");
            }
        }


        if (Kart.LapController.enabled) UpdateLapTimes();

        var controller = Kart.Controller;
        if (controller.BoostTime > 0f)
        {
            if (controller.BoostTierIndex == -1) return;

            Color color = controller.driftTiers[controller.BoostTierIndex].color;
        }
        else
        {
            if (!controller.IsDrifting) return;

        }
    }


    private void UpdateLapTimes()
    {
        if (!Kart.LapController.Object || !Kart.LapController.Object.IsValid)
            return;
        var lapTimes = Kart.LapController.LapTicks;
        for (var i = 0; i < Mathf.Min(lapTimes.Length, lapTimeTexts.Length); i++)
        {
            var lapTicks = lapTimes.Get(i);

            if (lapTicks == 0)
            {
                lapTimeTexts[i].text = "";
            }
            else
            {
                var previousTicks = i == 0
                    ? Kart.LapController.StartRaceTick
                    : lapTimes.Get(i - 1);

                var deltaTicks = lapTicks - previousTicks;
                var time = TickHelper.TickToSeconds(Kart.Runner, deltaTicks);

                SetLapTimeText(time, i);
            }
        }

        SetRaceTimeText(Kart.LapController.GetTotalRaceTime());
    }


    public void SetRaceTimeText(float time)
    {
        raceTimeText.text = $"{(int)(time / 60):00}:{time % 60:00.000}";
    }

    public void SetLapTimeText(float time, int index)
    {
        lapTimeTexts[index].text = $"<color=#FFC600>L{index + 1}</color> {(int)(time / 60):00}:{time % 60:00.000}";
    }

    public void ShowEndRaceScreen()
    {
        endRaceScreen.gameObject.SetActive(true);
    }

    // UI Hook
    public void OpenPauseMenu()
    {
        InterfaceManager.Instance.OpenPauseMenu();
    }
}