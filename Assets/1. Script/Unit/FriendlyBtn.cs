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
    Player player;


    bool set;
    Unit unit;
    bool mine;

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        if (set)
        {
            if (mine)
                inventory.friendlyHpText.text = "Hp : " + unit.hp.ToString() + "/" + maxHpText;
            if (unit == null)
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
        atkText = unit.GetComponentInChildren<Weapon>().damage.ToString();
    }

    public void OnclickedFriendlyBtn()
    {
        inventory.friendlyImageBasic.gameObject.SetActive(false);
        inventory.friendlyImage.gameObject.SetActive(true);
        inventory.friendlyImage.sprite = unitImage;

        inventory.friendlyAtkText.text = "Atk : " + atkText;

        for (int i = 0; i < inventory.changeStateBtns.Length; i++)
        {
            inventory.changeStateBtns[i].unit = unit;
            inventory.changeStateBtns[i].ChangeState(unit.GetComponent<RegularUnitBehaviour>());
            inventory.changeStateBtns[i].stateText = inventory.stateText;
        }
        mine = true;
        for (int i = 0; i < player.friendlyBtns.Count; i++)
        {
            if (player.friendlyBtns[i] == this)
            {
                continue;
            }
            else
            {
                player.friendlyBtns[i].mine = false;
            }
        }


    }

}
