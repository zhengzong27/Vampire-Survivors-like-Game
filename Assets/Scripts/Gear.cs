using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
 public ItemData.ItemType type;
 public float rate;
 public void Init(ItemData data){
    //Basic set
    name= "Gear " + data.itemId;
    transform.parent=GameManager.instance.player.transform;
    transform.localPosition=Vector3.zero;
    //Property set
    type=data.itemType;
    rate=data.damages[0];
    ApplyGear();
    }
    public void Levelup(float rate){
        this.rate=rate;
        ApplyGear();
    }
    void ApplyGear(){
        switch(type){
            case ItemData.ItemType.Glove:
            Rateup();
            break;
             case ItemData.ItemType.Shoe:
            Speedup();
            break;
        }
    }
    void Rateup()
    {
        Weapon[] weapons=transform.parent.GetComponentsInChildren<Weapon>();
        foreach(Weapon weapon in weapons){
            switch(weapon.id){
                case 0:
                weapon.speed=150+(150*rate);
                break;
                default:
                weapon.speed=0.5f+(1f-rate);
                break;
            }
        }
    }
    void Speedup(){
        float baseSpeed = 3f;  // 基础速度
        float speedIncrease = baseSpeed * rate;  // 速度增加值
        GameManager.instance.player.speed = baseSpeed + speedIncrease;  // 最终速度 = 基础速度 + 增加值
    }
}
