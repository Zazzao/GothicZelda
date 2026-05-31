using UnityEngine;


[System.Serializable]
public class Item {
    public ItemData itemData;
    public int quantity;

    public Item(ItemData itemData, int quantity) {
        this.itemData = itemData;
        this.quantity = quantity;
    }
}
