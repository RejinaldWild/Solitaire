using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Prospector : MonoBehaviour
{
    public static Prospector S;

    [Header("Set in Inspector")]
    public TextAsset deckJSON;
    public TextAsset layoutJSON;
    public float xOffset = 3f;
    public float yOffset = -2.5f;
    public Vector3 layoutCenter;

    [Header("Set Dynamically")]
    public Deck deck;
    public Layout layout;
    public List<CardProspector> drawPile;
    public Transform layoutAnchor;
    public CardProspector target;
    public List<CardProspector> tableau;
    public List<CardProspector> discardPile;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        deck = GetComponent<Deck>();
        deck.InitDeck(deckJSON.text);
        Deck.Shuffle(ref deck.cards);
        layout = GetComponent<Layout>();
        layout.ReadLayout(layoutJSON.text);
        drawPile = ConvertListCardsToListCardProspectors(deck.cards);
        LayoutGame();
    }

    private List<CardProspector> ConvertListCardsToListCardProspectors(List<Card> lCard)
    {
        List<CardProspector> lCProspector = new List<CardProspector>();
        CardProspector tCP;
        foreach( Card card in lCard)
        {
            tCP = card as CardProspector;
            lCProspector.Add(tCP);
        }

        return lCProspector;
    }

    private CardProspector Draw()
    {
        CardProspector cd = drawPile[0];
        drawPile.RemoveAt(0);
        return cd;
    }

    private void LayoutGame()
    {
        if(layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }

        CardProspector cp;
        foreach(SlotDef item in layout.slots)
        {
            cp = Draw();
            cp.faceUp = item.faceUp;
            cp.transform.parent = layoutAnchor;
            cp.transform.localPosition = new Vector3(
                layout.multiplier.x*item.x,
                layout.multiplier.y*item.y,
                -item.layerID);
            cp.layoutID = item.id;
            cp.slot = item;
            cp.state = eCardProspector.tableau;
            cp.SetSortingLayerName(item.layerName);
            tableau.Add(cp);
        }

        foreach(CardProspector tCP in tableau)
        {
            foreach(int hid in tCP.slot.hiddenBy)
            {
                cp = FindCardByLayoutID(hid);
                tCP.hiddenBys.Add(cp);
            }
        }

        MoveToTarget(Draw());
        UpdateDrawPile();
    }

    private CardProspector FindCardByLayoutID(int layoutID)
    {
        foreach(CardProspector tCP in tableau)
        {
            if(layoutID == tCP.layoutID)
            {
                return tCP;
            }
        }
        return null;
    }

    private void SetTableauFaces()
    {
        foreach(CardProspector card in tableau)
        {
            bool faceUp = true;
            foreach(CardProspector cover in card.hiddenBys)
            {
                if(cover.state == eCardProspector.tableau)
                {
                    faceUp = false;
                }
            }
            card.faceUp = faceUp;
        }
    }

    public void CardClicked(CardProspector card)
    {
        switch (card.state)
        {
            case eCardProspector.target:                
                break;
            case eCardProspector.drawpile:
                MoveToDiscard(target);
                MoveToTarget(Draw());
                UpdateDrawPile();
                break;
            case eCardProspector.tableau:
                bool validMatch = true;
                if (!card.faceUp)
                {
                    validMatch = false;
                }
                if(!AdjacentRank(card, target))
                {
                    validMatch = false;
                }
                if (!validMatch) return;
                tableau.Remove(card);
                MoveToTarget(card);
                SetTableauFaces();
                break;
        }

        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        if (tableau.Count == 0)
        {
            GameOver(true);
            return;
        }
        if (drawPile.Count > 0)
        {
            return;
        }
        foreach(CardProspector card in tableau)
        {
            if(AdjacentRank(card, target))
            {
                return;
            }
        }
        GameOver(false);
    }

    private void GameOver(bool check)
    {
        if (check)
        {
            print("Game Over! You won!");
        }
        else
        {
            print("Game Over! You lose!");
        }
        SceneManager.LoadScene("__Prospector_Scene_0");
    }

    private bool AdjacentRank(CardProspector card0, CardProspector card1)
    {
        if(!card0.faceUp || !card1.faceUp) return false;
        if (Mathf.Abs(card0.rank - card1.rank) == 1) return true;
        if(card0.rank == 1 && card1.rank == 13) return true;
        if (card0.rank == 13 && card1.rank == 1) return true;
        return false;
    }

    private void MoveToDiscard(CardProspector cd)
    {
        cd.state = eCardProspector.discard;
        discardPile.Add(cd);
        cd.transform.parent = layoutAnchor;

        cd.transform.localPosition = new Vector3(
            layout.multiplier.x * layout.discardPile.x,
            layout.multiplier.y * layout.discardPile.y,
            -layout.discardPile.layerID + 0.5f);
        cd.faceUp = true;
        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(-100 + discardPile.Count);
    }

    private void MoveToTarget(CardProspector cd)
    {
        if(target!=null) MoveToDiscard(cd);
        target = cd;
        cd.state = eCardProspector.target;
        cd.transform.parent = layoutAnchor;

        cd.transform.localPosition = new Vector3(
            layout.multiplier.x * layout.discardPile.x,
            layout.multiplier.y * layout.discardPile.y,
            -layout.discardPile.layerID);
        cd.faceUp = true;
        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(0);
    }

    private void UpdateDrawPile()
    {
        CardProspector cd;
        for(int i=0; i<drawPile.Count; i++)
        {
            cd = drawPile[i];
            cd.transform.parent = layoutAnchor;

            Vector2 dpStagger = layout.drawPile.stagger;
            cd.transform.localPosition = new Vector3(
            layout.multiplier.x * (layout.discardPile.x + i*dpStagger.x),
            layout.multiplier.y * (layout.discardPile.y + i * dpStagger.y),
            -layout.discardPile.layerID+0.1f*i);
            cd.faceUp = false;
            cd.SetSortingLayerName(layout.discardPile.layerName);
            cd.SetSortOrder(-10*i);
        }
    }
}
