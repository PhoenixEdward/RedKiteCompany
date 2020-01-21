using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

 namespace RedKite
{ 
    public class RegenLevel : MonoBehaviour, IPointerClickHandler
    { 

        public void OnPointerClick(PointerEventData data)
        {
            if (name == "Regenerate")
                Level.Instance.Regen();
        }
    }
}
