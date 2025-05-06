using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;   
    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;
    void Awake()
    {
        icon=GetComponentsInChildren<Image>()[1];
        icon.sprite=data.itemIcon;
        Text[] texts=GetComponentsInChildren<Text>();
        textLevel=texts[0];
        textName=texts[1];
        textDesc=texts[2];
        textName.text=data.itemName;
    }
    void OnEnable()
    { switch(data.itemType){
    case ItemData.ItemType.Melee:
    textLevel.text = "Lv." + (level + 1);
    textDesc.text=string.Format(data.itemDesc,data.damages[level],data.counts[level]);
    break;  
    case ItemData.ItemType.Glove:
    case ItemData.ItemType.Shoe:
    textLevel.text = "Lv." + (level + 1);
    textDesc.text=string.Format(data.itemDesc,data.damages[level]*100);
    break;
    default:
    textDesc.text=string.Format(data.itemDesc);
    break;
    }}
    public void OnClick()
    {
        switch(data.itemType){
            case ItemData.ItemType.Melee:
            if(level==0){
                GameObject newWeap=new GameObject();
                weapon=newWeap.AddComponent<Weapon>();
                weapon.init(data);
            }
            else{
                float nextDamage = data.baseDamage;
                int nextCount = data.baseCount;  // 从基础数量开始
                nextDamage += data.baseDamage * data.damages[level];
                nextCount += data.counts[level];
                weapon.Levelup(nextDamage, nextCount);
            }
            break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
            if(level==0){
                GameObject newGear=new GameObject();
                gear=newGear.AddComponent<Gear>();
                gear.Init(data);
            }
            else{
                float nextRate=data.damages[level];
                gear.Levelup(nextRate); 
            }
            break;
            case ItemData.ItemType.Heal:
            GameManager.instance.health=GameManager.instance.maxHealth;
            break;
        }
        level++;
        if(level==data.damages.Length){
            GetComponent<Button>().interactable=false;
        }
    }
}
