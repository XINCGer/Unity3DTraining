
public class Consumable : Item {
    public int BackHp { get; private set; }
    public int BackMp { get; private set; }
    public Consumable(int id, string name, string description, int buyPrice, int sellPrice, string Icon, int backHp, int backMp)
        : base(id, name, description, buyPrice, sellPrice, Icon) {
        this.BackHp = backHp;
        this.BackMp = backMp;
        base.ItemType = "Consumable";
    }
}