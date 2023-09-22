using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonLayout
{
    public Multiplier multiplier { get; set; }
    public Slot[] slots { get; set; }
    public Drawpile drawPile { get; set; }
    public Discardpile discardPile { get; set; }


    public class Multiplier
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    public class Drawpile
    {
        public int x { get; set; }
        public int y { get; set; }
        public string layer { get; set; }
        public float xStagger { get; set; }
    }

    public class Discardpile
    {
        public int x { get; set; }
        public int y { get; set; }
        public string layer { get; set; }
        public int xStagger { get; set; }
    }

    public class Slot
    {
        public int id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public bool faceUp { get; set; }
        public string layer { get; set; }
        public string hiddenByString { get; set; }
    }

}
