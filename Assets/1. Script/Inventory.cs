using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private static Inventory instance;
    public static Inventory Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        selectImage.gameObject.SetActive(false);
        selectDescription.gameObject.SetActive(false);
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
    }
    public Player player;

    public List<ItemFrame> itemFrames = new List<ItemFrame>();

    public Dictionary<string, InventoryItem> items = new Dictionary<string, InventoryItem>();

    public Weapon currentWeapon;

    public string weaponKey;

    public List<Button> buttons = new List<Button>();

    public List<Weapon> weapones = new List<Weapon>();
    public Image selectImage;
    public TMP_Text selectDescription;
    public void AddItem(string key, Sprite sprite, string description)
    {
        if (items.ContainsKey(key) == false)
        {
            items.Add(key, new InventoryItem(key, sprite, description));
        }

        items[key].count++;


        UpdateUI();
    }

    private void UpdateUI()
    {
        if (!gameObject.activeSelf)
            return;

        for (int i = 0; i < itemFrames.Count; i++)
            itemFrames[i].SetInventoryItem(null);

        int frameIdx = 0;
        foreach (var item in items)
        {
            itemFrames[frameIdx].SetInventoryItem(item.Value);
            frameIdx++;
        }
    }

    public void OnClickedSelectWeaponBtn()
    {
        
        foreach (Weapon weapon in weapones)
        {
            if(weapon.key == weaponKey)
            {
                weapon.gameObject.SetActive(true);
                currentWeapon = weapon;
            }
            else
            {
                weapon.gameObject.SetActive(false);
                currentWeapon = null;
            }
        }
        selectImage.gameObject.SetActive(false);
        selectDescription.gameObject.SetActive(false);
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.IsStop = false;
        player.inventoryBg.SetActive(false);
    }
}

[System.Serializable]
public class InventoryItem
{
    public InventoryItem(string name, Sprite sprite, string description)
    {
        this.name = name;
        this.sprite = sprite;
        this.description = description;
    }
    public Sprite sprite;
    public string name;
    public string description;
    public int count;
}
