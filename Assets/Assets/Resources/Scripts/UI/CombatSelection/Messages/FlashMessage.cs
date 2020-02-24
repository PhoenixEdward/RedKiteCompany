using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RedKite
{ 
    [Serializable]
    public class FlashMessage
    {
        public string message;
        public Color32 color;

        public FlashMessage() { }

        public FlashMessage(string _message, Color32 _color)
        {
            message = _message;
            color = _color;
        }
    }
}
