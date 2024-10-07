using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemFrame : MonoBehaviour
{
    public string key;
    public Image weaponImage;
    public string weaponName;
    public string weaponDescription;

    [HideInInspector]
    public InventoryItem inventoryItem;

    Inventory inventory;

    private void Start()
    {
        inventory = GetComponentInParent<Inventory>();
    }


    public void OnClickedSelectWeaponBtn()
    {
        Inventory weaponInventory = GameMgr.Instance.inventory;
        Debug.Log("OnClickedSelectWeaponBtn");
        if (!weaponInventory.selectImage.gameObject.activeSelf)
        {
            weaponInventory.selectImage.gameObject.SetActive(true);
            weaponInventory.selectDescription.gameObject.SetActive(true);
            for (int i = 0; i < weaponInventory.buttons.Count; i++)
            {
                weaponInventory.buttons[i].gameObject.SetActive(true);
            }
        }
        weaponInventory.selectImage.sprite = weaponImage.sprite;
        weaponInventory.selectDescription.text = weaponDescription;
        weaponInventory.weaponKey = key;
        weaponInventory.selectName.text = key;
        inventory.WeaponImageChange(weaponInventory.selectImage.sprite);
    }

    public void SetInventoryItem(InventoryItem item)
    {
        if (item == null)
        {
            inventoryItem = null;
            key = null;
            return;
        }
        else
        {
            inventoryItem = item;
            weaponImage.sprite = item.sprite;
            weaponDescription = item.description;
            key = inventoryItem.name;
        }
        
    }
}
