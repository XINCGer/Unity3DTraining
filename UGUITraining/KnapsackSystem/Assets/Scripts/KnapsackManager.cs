using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class KnapsackManager : MonoBehaviour {

    private Dictionary<int, Item> ItemList;
    private static KnapsackManager _instance;
    public GridPanelUI GridPanelUi;
    public TooltipsUI TooltipsUI;
    private bool isShow = false;
    public static KnapsackManager Instance {
        get { return _instance; }
    }

    public void Awake() {
        _instance = this;
        Load();
        //事件注册
        GridUI.OnEnter += GridUI_OnEnter;
        GridUI.OnExit += GridUI_OnExit;
    }

    void Update()
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.Find("Canvas").transform as RectTransform,
            Input.mousePosition, null, out position);
        if (isShow)
        {
            TooltipsUI.Show();
            TooltipsUI.SetLocalPosition(position);
        }
    }

    #region 事件回调
    private void GridUI_OnExit() {
        TooltipsUI.Hide();
        isShow = false;
    }

    private void GridUI_OnEnter(Transform gridTransform) {
        Item item = ItemModel.GetItem(gridTransform.name);
        if (item == null) return;
        else
        {
            string text = GetTooltipText(item);
            TooltipsUI.UpdateTooltip(text);
        }
        isShow = true;
    }


    #endregion
    private string GetTooltipText(Item item) {
        if (item == null)
            return "";
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("<color=red>{0}</color>\n\n", item.Name);
        switch (item.ItemType) {
            case "Armor":
                Armor armor = item as Armor;
                sb.AppendFormat("力量:{0}\n防御:{1}\n敏捷:{2}\n\n", armor.Power, armor.Defend, armor.Agility);
                break;
            case "Consumable":
                Consumable consumable = item as Consumable;
                sb.AppendFormat("HP:{0}\nMP:{1}\n\n", consumable.BackHp, consumable.BackMp);
                break;
            case "Weapon":
                Weapon weapon = item as Weapon;
                sb.AppendFormat("攻击:{0}\n\n", weapon.Damage);
                break;
            default:
                break;
        }
        sb.AppendFormat("<size=25><color=white>购买价格：{0}\n出售价格：{1}</color></size>\n\n<color=yellow><size=20>描述：{2}</size></color>", item.BuyPrice, item.SellPrice, item.Description);
        return sb.ToString();
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
        ItemModel.StoreItem(emptyGrid.name, temp);
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
