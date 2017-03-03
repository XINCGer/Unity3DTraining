using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnapsackManager : MonoBehaviour {

    private Dictionary<int, Item> ItemList;
    private static KnapsackManager _instance;
    public GridPanelUI GridPanelUi;

    public static KnapsackManager Instance {
        get { return _instance; }
    }

    public void Awake() {
        _instance = this;
        Load();
    }

    public void StoreItem(int itemId) {
        if (!ItemList.ContainsKey(itemId))
            return;
        Transform emptyGrid = GridPanelUi.GetComponent<GridPanelUI>().GetEmpty();
        if (emptyGrid == null) {
            Debug.LogWarning("背包已满！");
            return;
        }
        Item temp = ItemList[itemId];
        GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/Item");
        itemPrefab.GetComponent<ItemUI>().UpdateText(temp.Name);
        GameObject itemGameObject = GameObject.Instantiate(itemPrefab);
        itemGameObject.transform.SetParent(emptyGrid);
        itemGameObject.transform.localPosition = Vector3.zero;
        itemGameObject.transform.localScale = Vector3.one;

        //信息存储
        ItemModel.StoreItem(emptyGrid.name,temp);
    }

    //模拟数据加载的过程
    private void Load() {
        ItemList = new Dictionary<int, Item>();
        Weapon w1 = new Weapon(0, "牛刀", "牛B的刀！", 20, 10, "", 100);
        Weapon w2 = new Weapon(1, "羊刀", "杀羊刀。", 15, 10, "", 20);
        Weapon w3 = new Weapon(2, "宝剑", "大宝剑！", 120, 50, "", 500);
        Weapon w4 = new Weapon(3, "军枪", "可以对敌人射击，很厉害的一把枪。", 1500, 125, "", 720);

        Consumable c1 = new Consumable(4, "红瓶", "加血", 25, 11, "", 20, 0);
        Consumable c2 = new Consumable(5, "蓝瓶", "加蓝", 39, 19, "", 0, 20);

        Armor a1 = new Armor(6, "头盔", "保护脑袋！", 128, 83, "", 5, 40, 1);
        Armor a2 = new Armor(7, "护肩", "上古护肩，锈迹斑斑。", 1000, 0, "", 15, 40, 11);
        Armor a3 = new Armor(8, "胸甲", "皇上御赐胸甲。", 153, 0, "", 25, 30, 11);
        Armor a4 = new Armor(9, "护腿", "预防风寒，从腿做起", 999, 60, "", 19, 30, 51);

        ItemList.Add(w1.ID, w1);
        ItemList.Add(w2.ID, w2);
        ItemList.Add(w3.ID, w3);
        ItemList.Add(w4.ID, w4);
        ItemList.Add(c1.ID, c1);
        ItemList.Add(c2.ID, c2);
        ItemList.Add(a1.ID, a1);
        ItemList.Add(a2.ID, a2);
        ItemList.Add(a3.ID, a3);
        ItemList.Add(a4.ID, a4);
    }
}
