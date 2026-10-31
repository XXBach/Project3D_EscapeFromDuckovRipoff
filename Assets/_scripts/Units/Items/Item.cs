using UnityEngine;

public enum Rarity
{
    Normal = 0,
    Rare = 1,
    SuperRare = 2,
    EndGame = 3
}
[System.Serializable]
public abstract class Item
{
    public string Name { get; private set; }
    public Rarity Scarcity { get; private set; }
    public Domain OwnerDomain { get; private set; }
    
    public Item()
    {
        Name = string.Empty;
        Scarcity = Rarity.Normal;
        OwnerDomain = Domain.None;
    }

    public Item(string name, Rarity scarcity, Domain ownerDomain)
    {
        Name = name;
        Scarcity = scarcity;
        OwnerDomain = ownerDomain;
    }
    public Item(string name, Rarity scarcity)
    {
        Name = name;
        Scarcity = scarcity;
        OwnerDomain = Domain.None;
    }
    public Item(string name, Domain ownerDomain)
    {
        Name = name;
        Scarcity = Rarity.Normal;
        OwnerDomain = ownerDomain;
    }
    public Item(Rarity scarcity, Domain ownerDomain)
    {
        Name = string.Empty;
        Scarcity = scarcity;
        OwnerDomain = ownerDomain;
    }
    public Item(string name)
    {
        Name = name;
    }
    public Item(Domain ownerDomain)
    {
        OwnerDomain = ownerDomain;
    }
    public Item(Rarity scarcity)
    {
        Scarcity = scarcity;
    }
    public Item(Item copyItem)
    {
        Name = copyItem.Name;
        Scarcity = copyItem.Scarcity;
        OwnerDomain = copyItem.OwnerDomain;
    }
}
