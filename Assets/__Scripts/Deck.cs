using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using Unity.VisualScripting;

public class Deck : MonoBehaviour
{
    private Sprite _tSp = null;
    private GameObject _tGO = null;
    private SpriteRenderer _tSR = null;

    [Header("Set in Inspector")]
    public bool startFaceUp = false;

    public Sprite suitClub;
    public Sprite suitDiamond;
    public Sprite suitHeart;
    public Sprite suitSpade;
    public Sprite cardBack;
    public Sprite cardBackGold; 
    public Sprite cardFront;    
    public Sprite cardFrontGold;
    
    public Sprite[] faceSprites;
    public Sprite[] rankSprites;

    public GameObject prefabCard;
    public GameObject prefabSprite;


    [Header("Set Dynamically")]
    public JsonModel model;
    public List<string> cardNames;
    public List<Card> cards;
    public List<Decorator> decorators;
    public List<CardDefinition> cardDefinitions;
    public Transform deckAnchor;
    public Dictionary<string, Sprite> dictSuits;
    
    public void InitDeck(string deckJSON)
    {
        if (GameObject.Find("_Deck") == null)
        {
            GameObject anchorGO = new GameObject("_Deck");
            deckAnchor = anchorGO.transform;
        }

        dictSuits = new Dictionary<string, Sprite>()
        {
            {"C",suitClub },
            {"D",suitDiamond},
            {"H",suitHeart},
            {"S", suitSpade }
        };

        ReadDeck(deckJSON);

        MakeCards();
    }

    public void ReadDeck(string deckJSON)
    {
        var model = JsonConvert.DeserializeObject<JsonModel>(deckJSON);
        string str = "decorators[0] ";
        str += "type= " + model.decorators[0].type;
        str += " x= " + model.decorators[0].loc.x;
        str += " y= " + model.decorators[0].loc.y;
        str += " scale= " + model.decorators[0].scale;
        //print(str);

        decorators = new List<Decorator>();
        var xDecos = model.decorators;
        Decorator deco;        

        for(int i = 0; i < xDecos.Length; i++)
        {
            deco = new Decorator();
            deco.type = xDecos[i].type;
            deco.flip = xDecos[i].flip;
            deco.scale = float.Parse(xDecos[i].scale.ToString());
            deco.location.x = xDecos[i].loc.x;
            deco.location.y = xDecos[i].loc.y;
            deco.location.z = xDecos[i].loc.z;
            decorators.Add(deco);
        }

        cardDefinitions = new List<CardDefinition>();
        var xCardDefs = model.cards;
        for(int i=0;i<xCardDefs.Length;i++)
        {
            CardDefinition cDef = new CardDefinition();
            cDef.rank = xCardDefs[i].rank;
            var xPips = xCardDefs[i].pips;
            if(xPips != null)
            {
                for(int j=0; j < xPips.Length; j++)
                {
                    deco = new Decorator();
                    deco.type = "pip";
                    deco.flip = xPips[j].flip;
                    deco.location.x = xPips[j].loc.x;
                    deco.location.y = xPips[j].loc.y;
                    deco.location.z = xPips[j].loc.z;                    
                    deco.scale = xPips[j].scale;
                    cDef.pips.Add(deco);
                }
            }
            cDef.face = xCardDefs[i].face;
            cardDefinitions.Add(cDef);
        }
    }

    public CardDefinition GetCardDefinitionsByRank (int rankCard)
    {
        foreach(CardDefinition cDef in cardDefinitions)
        {
            if(cDef.rank == rankCard)
            {
                return cDef;
            }
        }
        return null;
    }
    
    public void MakeCards()
    {
        cardNames = new List<string>();
        string[] letters = new string[] { "C", "D", "H", "S" };
        foreach (string l in letters)
        {
            for(int i = 0; i < 13; i++)
            {
                cardNames.Add(l+(i+1));
            }
        }

        cards = new List<Card>();

        for(int i = 0; i< cardNames.Count;i++)
        {
            cards.Add(MakeCard(i));
        }
    }

    private Card MakeCard(int cardNum)
    {
        GameObject cgo = Instantiate(prefabCard) as GameObject;
        cgo.transform.parent = deckAnchor;
        Card card = cgo.GetComponent<Card>();

        cgo.transform.localPosition = new Vector3((cardNum % 13) * 3, cardNum / 13 * 4, 0);

        card.name = cardNames[cardNum];
        card.suit = card.name[0].ToString();
        card.rank = int.Parse(card.name.Substring(1));
        if(card.suit =="H"|| card.suit == "D")
        {
            card.colS = "Red";
            card.color = Color.red;
        }

        card.def = GetCardDefinitionsByRank(card.rank);

        AddDecorators(card);
        AddPips(card);
        AddFace(card);
        AddBack(card);

        return card;
    }

    private void AddBack(Card card)
    {
        _tGO = Instantiate(prefabCard);
        _tSR = _tGO.GetComponent<SpriteRenderer>();
        _tSR.sprite = cardBack;
        _tGO.transform.SetParent(card.transform);
        _tGO.transform.localPosition = Vector3.zero;
        _tSR.sortingOrder = 2;
        _tSR.name = "back";
        card.back = _tGO;
        card.faceUp = startFaceUp;
    }

    private void AddPips(Card card)
    {
        foreach(Decorator pip in card.def.pips)
        {
            _tGO = Instantiate(prefabSprite);
            _tGO.transform.SetParent(card.transform);
            _tGO.transform.localPosition = pip.location;

            if (pip.flip)
            {
                _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            if (pip.scale != 1)
            {
                _tGO.transform.localScale = Vector3.one * pip.scale;
            }

            _tGO.name = "pip";
            _tSR = _tGO.GetComponent<SpriteRenderer>();
            _tSR.sprite = dictSuits[card.suit];
            _tSR.sortingOrder = 1;

            card.pipGOs.Add(_tGO);
        }
    }

    private void AddFace(Card card)
    {
        if(card.def.face == "")
        {
            return;
        }

        _tGO = Instantiate(prefabSprite);
        _tSR = _tGO.GetComponent<SpriteRenderer>();
        _tSp = GetFace(card.def.face + card.suit);
        _tSR.sprite = _tSp;
        _tSR.sortingOrder = 1;
        _tGO.transform.SetParent(card.transform);
        _tGO.transform.localPosition = Vector3.zero;
        _tGO.name = "face";
    }

    private Sprite GetFace(string faceS)
    {
        foreach(Sprite _tSP in faceSprites)
        {
            if(_tSP.name == faceS)
            {
                return _tSP;
            }
        }
        return null;
    }

    private void AddDecorators(Card card)
    {
        foreach(Decorator deco in decorators)
        {
            if(deco.type == "suit")
            {
                _tGO = Instantiate(prefabSprite);
                _tSR = _tGO.GetComponent<SpriteRenderer>();
                _tSR.sprite = dictSuits[card.suit];
            }
            else
            {
                _tGO = Instantiate(prefabSprite);
                _tSR = _tGO.GetComponent<SpriteRenderer>();
                _tSp = rankSprites[card.rank];
                _tSR.sprite = _tSp;
                _tSR.color = card.color;
            }

            _tSR.sortingOrder = 1;
            _tGO.transform.SetParent(card.transform);
            _tGO.transform.localPosition = deco.location;
            if (deco.flip)
            {
                _tGO.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            if (deco.scale != 1)
            {
                _tGO.transform.localScale = Vector3.one * deco.scale;
            }

            _tGO.name = deco.type;
            card.decoGOs.Add(_tGO);
        }
    }

    public static void Shuffle(ref List<Card> cards)
    {
        List<Card> tCards = new List<Card>();
        int index;
        
        while( cards.Count > 0)
        {
            index = Random.Range(0, cards.Count);
            tCards.Add(cards[index]);
            cards.RemoveAt(index);
        }
        cards = tCards;
    }

}