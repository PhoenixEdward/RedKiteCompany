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
        Toggle mirrored;
        public SpriteSelection spriteSelector;
        bool firstClick = true;

        public void Start()
        {
            dropdown = GetComponent<Dropdown>();
            mirrored = GetComponentInChildren<Toggle>();
            spriteSelector = GetComponentInParent<SpriteSelection>();
        }

        void OnGUI()
        {
        }

        public void OnSelect(BaseEventData baseEvent)
        {
            //identities are base on order from top to bottom in GUI menu.
            if (Convert.ToInt32(gameObject.name) != 3 & Convert.ToInt32(gameObject.name) != 4)
                StartCoroutine(spriteSelector.GetTextures(dropdown.options[dropdown.value].text, Convert.ToInt32(gameObject.name), mirrored.isOn));
            else
                StartCoroutine(spriteSelector.GetTextures(dropdown.options[dropdown.value].text, Convert.ToInt32(gameObject.name), false));

        }


    }
}