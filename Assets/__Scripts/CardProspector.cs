using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardProspector
{
    drawpile,
    tableau,
    target,
    discard
}

public class CardProspector : Card
{

    [Header("Set Dynamically")]
    public eCardProspector state = eCardProspector.drawpile;
    public List<CardProspector> hiddenBys = new List<CardProspector>();
    public int layoutID;
    public SlotDef slot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnMouseUpAsButton()
    {
        Prospector.S.CardClicked(this);
        base.OnMouseUpAsButton();
    }

}
