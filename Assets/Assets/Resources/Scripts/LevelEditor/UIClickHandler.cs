using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RedKite
{
    public class UIClickHandler : MonoBehaviour, IPointerClickHandler
    {
        Level level;
        // Start is called before the first frame update
        void Start()
        {
            level = FindObjectOfType<Level>();
        }

        public void OnPointerClick(PointerEventData data)
        {
            if (name == "Regenerate")
                level.Regen();
            else if (name == "Exit")
                Application.Quit();
        }
    }
}