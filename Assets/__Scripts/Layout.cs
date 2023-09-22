using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SlotDef
{
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>();
    public string type = "slot";
    public Vector2 stagger;
}


public class Layout : MonoBehaviour
{
    public JsonLayout layout;
    public Vector2 multiplier;
    public List<SlotDef> slots;
    public SlotDef drawPile;
    public SlotDef discardPile;
    public string[] sortingLayerNames = new string[] { "Row0", "Row1", 
                                    "Row2", "Row3", "Discard", "Draw" };


    public void ReadLayout(string JsonLayout)
    {
        layout = JsonConvert.DeserializeObject<JsonLayout>(JsonLayout);
        multiplier.x = layout.multiplier.x;
        multiplier.y = layout.multiplier.y;

        var items = layout.slots;
        
        for (int i = 0; i < items.Length; i++)
        {
            SlotDef tSD = new SlotDef();
            tSD.x = items[i].x;
            tSD.y = items[i].y;            
            tSD.layerName = items[i].layer;
            tSD.layerID = Array.FindIndex(sortingLayerNames, x => x == tSD.layerName);
            tSD.faceUp = items[i].faceUp;
            tSD.id = items[i].id;
            if (items[i].hiddenByString.Length>0)
            {
                string[] hiding = items[i].hiddenByString.Split(',');
                foreach (string str in hiding)
                {
                    tSD.hiddenBy.Add(Int32.Parse(str));                                  
                }
            }
            slots.Add(tSD);
        }

        SlotDef tSDDraw = new SlotDef();
        tSDDraw.x = layout.drawPile.x;
        tSDDraw.y = layout.drawPile.y;
        tSDDraw.layerName = layout.drawPile.layer;
        tSDDraw.stagger.x = layout.drawPile.xStagger;
        drawPile = tSDDraw;

        SlotDef tSDDP = new SlotDef();
        tSDDP.x = layout.discardPile.x;
        tSDDP.y = layout.discardPile.y;
        tSDDP.layerName = layout.discardPile.layer;
        tSDDP.stagger.x = layout.discardPile.xStagger;
        discardPile = tSDDP;
            
    }
}
