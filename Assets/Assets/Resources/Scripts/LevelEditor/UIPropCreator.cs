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
        public Vector3Int position;
        public bool IsActive { get; private set; }
        public SpriteSelection spriteSelector;

        // Start is called before the first frame update
        void Start()
        {
            propCreateDropDown = transform.parent.GetComponentInChildren<Dropdown>();
            propCreateToggle = transform.parent.GetComponentInChildren<Toggle>();
            spriteSelector = FindObjectOfType<SpriteSelection>();
        }

        // Update is called once per frame
        public void OnPointerClick(PointerEventData data)
        {
            if (name == "Create")
                StartCoroutine(spriteSelector.GetPropTextures(propCreateDropDown.options[propCreateDropDown.value].text, position, propCreateToggle.isOn));
            else if (name == "Delete")
                Level.Instance.RemoveProp(position);
        }
    }
}