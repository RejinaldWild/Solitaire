using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JsonModel
{
    public Decorator[] decorators { get; set; }    
    public Card[] cards { get; set; }
    
    public class Decorator
    {        
        public string type { get; set; }
        
        public Loc loc { get; set; }
        
        public bool flip { get; set; }
        
        public float scale { get; set; }
    }

    
    public class Loc
    {        
        public float x { get; set; }
        
        public float y { get; set; }
        
        public float z { get; set; }
    }
    
    public class Card
    {        
        public int rank { get; set; }
        
        public string face { get; set; }
        
        public Pip[] pips { get; set; }
    }
    
    public class Pip
    {
        
        public string type { get; set; }
        
        public Loc1 loc { get; set; }
        
        public bool flip { get; set; }
        
        public float scale { get; set; }
    }
    
    public class Loc1
    {
        
        public float x { get; set; }
        
        public float y { get; set; }
        
        public float z { get; set; }
    }

}
