using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace RedKite
{ 
    public class StatSheet : MonoBehaviour
    {
        RectTransform rect;
        StatText[] stats;
        HealthBar healthBar;
        bool isActive;
        UnitSprite unitSprite;

        void Start()
        {
            stats = GetComponentsInChildren<StatText>();
            healthBar = GetComponentInChildren<HealthBar>();
            unitSprite = GetComponentInChildren<UnitSprite>();
            rect = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        public void Activate(Unit unit)
        {
            rect.localPosition = new Vector2(-Screen.width/2, -Screen.height/2);
            isActive = true;

            foreach (StatText stat in stats)
                stat.unit = unit;

            healthBar.SelectedUnit = unit;

            unitSprite.selectedUnit = unit;
            
        }

        //potentially clear above data.
        public void Deactivate()
        {
            rect.localPosition = new Vector2(-Screen.width / 2, -Screen.height / 2 - rect.sizeDelta.y);
            isActive = false;
        }
    }
}