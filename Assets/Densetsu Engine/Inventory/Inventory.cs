using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    
    public List<Item> items = new List<Item>();

    public void AddItem(ItemData itemData) {

        // if not stackable just add item
        if (!itemData.isStackable) {
            items.Add(new Item(itemData, 1));
            return;
        }

        //else look for existing stack
        foreach (Item item in items) {
            if (item.itemData == itemData && item.quantity < itemData.maxStackSize) {
                item.quantity++;
                return;
            }
        
        }

        // if no stack found start new stack
        items.Add(new Item(itemData, 1));


    }

    
}
