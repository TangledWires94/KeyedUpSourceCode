using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatingObject : MonoBehaviour
{
    public void StartGame()
    {
        Manager<GameManager>.Instance.RunGame();
    }

    public void ShowContinue()
    {
        Manager<UIManager>.Instance.ShowContinue();
        Manager<GameManager>.Instance.SubscribeToContinue();
    }

    public void ChangeLivesNumber()
    {
        int remainingLives = Manager<GameManager>.Instance.livesRemaining;
        Manager<UIManager>.Instance.SetLivesNumber(remainingLives);
    }

    public void ChangeScoreNumber()
    {
        int score = Manager<GameManager>.Instance.levelsCompleted;
        Manager<UIManager>.Instance.SetScoreNumber(score);
    }

    public void FreezeTime()
    {
        Time.timeScale = 0;
    }

    public void PlaySoundEffect(SoundManager.SoundEffect soundEffect)
    {
        Manager<SoundManager>.Instance.PlaySoundEffect(soundEffect);
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
