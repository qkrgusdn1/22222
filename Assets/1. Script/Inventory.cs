using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private void Awake()
    {
        selectImage.gameObject.SetActive(false);
        selectDescription.gameObject.SetActive(false);
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
    }

    public GameObject itemMenu;
    public GameObject friendlyMenu;
    public TMP_Text menuText;

    public Player player;

    public List<ItemFrame> itemFrames = new List<ItemFrame>();

    public Dictionary<string, InventoryItem> items = new Dictionary<string, InventoryItem>();

    public Weapon currentWeapon;

    public string weaponKey;

    public List<Button> buttons = new List<Button>();

    public List<Weapon> weapones = new List<Weapon>();
    public Image selectImage;
    public TMP_Text selectDescription;
    public TMP_Text selectName;
    public TMP_Text selectAtkDescription;

    public GameObject friendlyBtnGroup;

    public Image friendlyImage;
    public Image friendlyImageBasic;
    public TMP_Text friendlyHpText;
    public TMP_Text friendlyAtkText;

    public bool isItemMenu;

    public Image weaponImage;

    Sprite weaponSprite;

    public Sprite weaponFrameBgSprite;

    public ChangeStateBtn[] changeStateBtns;
    public TMP_Text stateText;


    private void Start()
    {
        isItemMenu = true;
    }

    public void OnClickedRightBtn()
    {
        isItemMenu = false;
    }

    public void OnClickedLeftBtn()
    {
        isItemMenu = true;
    }

    private void Update()
    {
        if (isItemMenu)
        {
            itemMenu.gameObject.SetActive(true);
            friendlyMenu.gameObject.SetActive(false);
            menuText.text = "아이템";
        }
        else
        {
            itemMenu.gameObject.SetActive(false);
            friendlyMenu.gameObject.SetActive(true);
            menuText.text = "아군";
        }
    }

    public void AddItem(string key, Sprite sprite, string description, string atkatkDescription)
    {
        if (items.ContainsKey(key) == false)
        {
            items.Add(key, new InventoryItem(key, sprite, description, atkatkDescription));
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

    public void WeaponImageChange(Sprite weaponSprite)
    {
        this.weaponSprite = weaponSprite;
    }

    public void OnClickedSelectWeaponBtn()
    {
        foreach (Weapon weapon in weapones)
        {
            if(weapon.key == weaponKey)
            {
                weapon.gameObject.SetActive(true);
                currentWeapon = weapon;
                weaponImage.sprite = weaponSprite;
            }
            else
            {
                weapon.gameObject.SetActive(false);
                weaponImage.sprite = weaponFrameBgSprite;
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
    public InventoryItem(string name, Sprite sprite, string description, string atkDescription)
    {
        this.name = name;
        this.sprite = sprite;
        this.description = description;
        this.atkDescription = atkDescription;
    }
    public Sprite sprite;
    public string name;
    public string description;
    public string atkDescription;
    public int count;
}

