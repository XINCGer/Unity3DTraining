
public class Armor : Item {
    public int Power { get; private set; }
    public int Defend { get; private set; }
    public int Agility { get; private set; }
    public Armor(int id, string name, string description, int buyPrice, int sellPrice, string Icon, int power, int defend, int agility)
        : base(id, name, description, buyPrice, sellPrice, Icon) {
        this.Power = power;
        this.Defend = defend;
        this.Agility = agility;
        base.ItemType = "Armor";
    }
}