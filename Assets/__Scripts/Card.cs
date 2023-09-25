using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Set Dynamically")]
    public string suit;
    public int rank;
    public Color color = Color.black;
    public string colS = "Black";

    public List<GameObject> decoGOs = new List<GameObject>();
    public List<GameObject> pipGOs = new List<GameObject>();
    public GameObject back;
    public CardDefinition def;
    public SpriteRenderer[] spriteRenderers;

    public bool faceUp
    {
        get
        {
            return !back.activeSelf;
        }
        set
        {
            back.SetActive(!value);
        }
    }
    
    virtual public void OnMouseUpAsButton()
    {
        print(name);
    }

    private void Start()
    {
        SetSortOrder(0);
    }

    public void SetSortOrder(int sOrder)
    {
        PopulateSpriteRenderers();

        foreach(SpriteRenderer sprite in spriteRenderers )
        {
            if(sprite.gameObject == this.gameObject)
            {
                sprite.sortingOrder = sOrder;
                continue;
            }
            switch (sprite.gameObject.name)
            {
                case "back":
                    sprite.sortingOrder = sOrder+2;
                    break;
                case "face":
                default:
                    sprite.sortingOrder = sOrder+1;
                    break;
            }                
        }

    }

    public void SetSortingLayerName(string name)
    {
        PopulateSpriteRenderers();

        foreach (SpriteRenderer sprite in  spriteRenderers)
        {
            sprite.sortingLayerName = name;
        }
    }

    public void PopulateSpriteRenderers()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
    }
}

[System.Serializable]
public class Decorator
{
    public string type;
    public Vector3 location;
    public bool flip = false;
    public float scale = 1f;
}

[System.Serializable]
public class CardDefinition
{
    public int rank;
    public string face;
    public List<Decorator> pips = new List<Decorator>();
}
