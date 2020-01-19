using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace RedKite
{
    public class UIElement : MonoBehaviour, ISelectHandler
    {
        Dropdown dropdown;
        public SpriteSelection spriteSelector;
        bool firstClick = true;

        public void Start()
        {
            dropdown = GetComponent<Dropdown>();
            spriteSelector = GetComponentInParent<SpriteSelection>();
        }

        void OnGUI()
        {
        }

        public void OnSelect(BaseEventData baseEvent)
        {
            //identities are base on order from top to bottom in GUI menu.
            if(!firstClick)
            { 
                StartCoroutine(spriteSelector.GetTextures(dropdown.options[dropdown.value].text, Convert.ToInt32(gameObject.name)));
                Debug.Log(Convert.ToInt32(gameObject.name));
                firstClick = true;
            }
            else
            {
                firstClick = false;
            }
        }


    }
}