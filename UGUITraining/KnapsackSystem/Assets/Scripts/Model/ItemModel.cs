using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemModel{

    private static Dictionary<string, Item> GridItem = new Dictionary<string, Item>();

    public static void StoreItem(string key, Item value) {
        if (GridItem.ContainsKey(key)) {
            GridItem.Remove(key);
        }
        GridItem.Add(key, value);
    }

    public static Item GetItem(string key) {
        if (GridItem.ContainsKey(key)) {
            return GridItem[key];
        }
        else {
            return null;
        }
    }

    public static void DeleteItem(string key) {
        if (GridItem.ContainsKey(key)) {
            GridItem.Remove(key);
        }
    }
}
