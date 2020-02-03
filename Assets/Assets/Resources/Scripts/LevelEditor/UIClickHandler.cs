using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RedKite
{
    public class UIClickHandler : MonoBehaviour, IPointerClickHandler
    {

        public void OnPointerClick(PointerEventData data)
        {
            if (name == "Regenerate")
                Level.Instance.Regen();
            else if (name == "Exit")
                Application.Quit();
        }
    }
}