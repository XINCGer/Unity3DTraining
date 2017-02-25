
using UnityEngine.iOS;

public class Weapon : Item {

    public int Damage { get; private set; }
    public Weapon(int id, string name, string description, int buyPrice, int sellPrice, string Icon, int damage)
        : base(id, name, description, buyPrice, sellPrice, Icon) {
        this.Damage = damage;
        base.ItemType = "Weapon";
    }
}