using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUD : MonoBehaviour
{
    public enum InfoType {Exp,Level,Kill,Time,Health }
    public InfoType type;
    Text myText;
    Slider mySlider;
    private void Awake()
    {
        myText = GetComponentInChildren<Text>();
        mySlider = GetComponentInChildren<Slider>();
    }
    private void LateUpdate()
    {
        if (GameManager.instance == null || !GameManager.instance.isInitialized)
            return;

        switch (type) {
            case InfoType.Exp:
                if (mySlider == null) return;
                float curExp = GameManager.instance.exp;
                if (GameManager.instance.nextExp == null || GameManager.instance.nextExp.Length == 0)
                    return;
                int levelIndex = Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1);
                float maxExp = GameManager.instance.nextExp[levelIndex];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                if (myText == null) return;
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level);
                break;
            case InfoType.Kill:
                if (myText == null) return;
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Time:
                if (myText == null) return;
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Health:
                if (mySlider == null) return;
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
        }
    }
}
