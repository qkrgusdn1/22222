using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string key;

    public Sprite sprite;

    public ItemType itemType;

    public float upDownSpeed;
    public float upDownDistance;

    public string description;

    public string atkDescription;

    public float rotationSpeed;

    public GameObject body;

    Vector3 initialPosition;
    void Start()
    {
        initialPosition = transform.position;
    }
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        float newY = Mathf.PingPong(Time.time * upDownSpeed, upDownDistance * 2);
        transform.position = new Vector3(initialPosition.x, initialPosition.y + newY, initialPosition.z);
    }
    public void GetItem()
    {
        if (itemType == ItemType.weapon)
        {
            for (int i = 0; i < GameMgr.Instance.inventory.itemFrames.Count; i++)
            {
                if (string.IsNullOrEmpty(GameMgr.Instance.inventory.itemFrames[i].key) && GameMgr.Instance.inventory.items.ContainsKey(key) == false)
                {
                    GameMgr.Instance.inventory.items.Add(key, new InventoryItem(key, sprite, description, atkDescription));
                    GameMgr.Instance.inventory.itemFrames[i].SetInventoryItem(new InventoryItem(key, sprite, description, atkDescription));
                    GameMgr.Instance.inventory.AddItem(key, sprite, description, atkDescription);
                    break;
                }
            }
        }
        else if(itemType == ItemType.heal)
        {

        }

        Destroy(gameObject);
    }
}

public enum ItemType
{
    weapon,
    heal
}
