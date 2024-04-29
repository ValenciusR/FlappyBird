using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuWindow : MonoBehaviour
{
    public void PlayBtn_OnClick()
    {
        Loader.Load(Loader.Scene.GameScene);
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
    }

    public void QuiBtn_OnClick()
    {
        Application.Quit();
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
    }
}
