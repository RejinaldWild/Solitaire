using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Prospector : MonoBehaviour
{
    public static Prospector S;

    [Header("Set in Inspector")]
    public TextAsset deckJSON;
    public TextAsset layoutJSON;

    [Header("Set Dynamically")]
    public Deck deck;
    public Layout layout;

    private void Awake()
    {
        S = this;
    }

    void Start()
    {
        deck = GetComponent<Deck>();
        deck.InitDeck(deckJSON.text);
        Deck.Shuffle(ref deck.cards);
        layout = GetComponent<Layout>();
        layout.ReadLayout(layoutJSON.text);

        //Card card;
        //for(int i = 0; i>deck.cards.Count; i++){
        //    card = deck.cards[i];
        //    card.transform.localPosition = new Vector3((i % 13) * 3, i / 13 * 4, 0);
        //}
    }

    
}
