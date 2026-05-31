using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
public class ItemData : ScriptableObject{

    public string itemName;
    public Sprite icon;
    public bool isStackable;
    public int maxStackSize = 1;

    //[TextArea]
    public string description;
    
}
