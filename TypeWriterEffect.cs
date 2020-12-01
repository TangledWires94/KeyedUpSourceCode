using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeWriterEffect : MonoBehaviour {
    public delegate void TypeWriterEvent();
    public event TypeWriterEvent OnFinished;

    public float TextDelay;  //speed of showing new character
    public string FullText;
    private string CurrentText = "";
    public TypeWriterEffect typeEffect;

    void Start()
    {
        if(typeEffect == null)
        {
            StartShowText();
        } else
        {
            typeEffect.OnFinished += StartShowText;
        }
    }

    void StartShowText()
    {
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText(){
        Manager<SoundManager>.Instance.ChangeBackgroundMusic(SoundManager.BackgroundMusic.Typing);
        for(int i = 0; i < FullText.Length; i++){
            CurrentText = FullText.Substring(0,i);
            this.GetComponent<Text>().text = CurrentText;
            yield return new WaitForSeconds(TextDelay);
        }
        Manager<SoundManager>.Instance.StopMusic();
        Manager<SoundManager>.Instance.PlaySoundEffect(SoundManager.SoundEffect.TypeWriterReturn);
        yield return new WaitForSeconds(1);
        if (OnFinished != null)
        {
            OnFinished.Invoke();
        }
    }

    void OnDisable()
    {
        OnFinished = null;
    }

}