using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEditor.Progress;

public class Item : MonoBehaviour
{
    public string key;

    public Sprite sprite;

    public ItemType itemType;

    public float upDownSpeed;
    public float upDownDistance;

    public string description;

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

            for (int i = 0; i < Inventory.Instance.itemFrames.Count; i++)
            {
                if (string.IsNullOrEmpty(Inventory.Instance.itemFrames[i].key))
                {
                    if (Inventory.Instance.items.ContainsKey(key) == false)
                    {
                        Inventory.Instance.items.Add(key, new InventoryItem(key, sprite, description));
                    }
                    Inventory.Instance.itemFrames[i].inventoryItem.name = key;
                    Inventory.Instance.AddItem(key, sprite, description);
                    break;
                }
            }
        }else if(itemType == ItemType.armor)
        {

        }

        Destroy(gameObject);
    }
}

public enum ItemType
{
    weapon,
    armor
}
