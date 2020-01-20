using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

namespace RedKite
{ 
    public class UIPropCreator : MonoBehaviour, IPointerClickHandler
    {
        Reticle reticle;
        Toggle propCreateToggle;
        public Dropdown propCreateDropDown;
        Button[] propCreateButtons;
        public Vector3 position;
        public bool IsActive { get; private set; }
        SpriteSelection spriteSelector;

        // Start is called before the first frame update
        void Start()
        {

            propCreateDropDown = GetComponentInChildren<Dropdown>();
            propCreateToggle = GetComponentInChildren<Toggle>();
            propCreateButtons = GetComponentsInChildren<Button>();
        }

        // Update is called once per frame
        public void OnPointerClick(PointerEventData data)
        {
            if(data.selectedObject.name == "Create")
            {
                spriteSelector.GetPorpTextures(propCreateDropDown.options[propCreateDropDown.value].text, position, propCreateToggle.isOn);
            }
        }
    }
}