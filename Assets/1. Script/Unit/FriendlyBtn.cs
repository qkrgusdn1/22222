using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendlyBtn : MonoBehaviour
{
    public string unitName;
    public TMP_Text unitNameText;
    public Image unitThumSprite;
    public Sprite unitImage;
    public string maxHpText;
    public string hpText;
    public string atkText;

    Inventory inventory;

    bool set;
    Unit unit;

    private void Update()
    {
        if (set)
        {
            inventory.friendlyHpText.text = "Hp : " + hpText + "/" + maxHpText;
            if(unit == null)
            {
                Destroy(gameObject);
            }
        }

    }

    public void SetFriendlyBtn(Inventory inventory, Unit unit)
    {
        this.unit = unit;
        set = true;
        unitName = unit.unitName;
        unitNameText.text = unitName;
        this.inventory = inventory;
        unitThumSprite.sprite = unit.thumSprite;
        unitImage = unit.image;
        maxHpText = unit.maxHp.ToString();
        hpText = unit.hp.ToString();
        atkText = unit.GetComponentInChildren<Weapon>().damage.ToString();
    }

    public void OnclickedFriendlyBtn()
    {
        inventory.friendlyImageBasic.gameObject.SetActive(false);
        inventory.friendlyImage.gameObject.SetActive(true);
        inventory.friendlyImage.sprite = unitImage;

        inventory.friendlyAtkText.text = "Atk : " + atkText;

    }

}
