using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RedKite
{ 
    public class HealthBar : MonoBehaviour
    {
        public Unit SelectedUnit { get; set; }
        Image image;
        // Start is called before the first frame update
        void Start()
        {
            image = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            if(SelectedUnit != null)
            { 
                image.fillAmount = ((float)SelectedUnit.Health / (float)SelectedUnit.MaxHealth);
                Debug.Log("Current Health: " + SelectedUnit.Health);
                Debug.Log("Max Health: " + SelectedUnit.MaxHealth);
                Debug.Log("Ratio: " + (SelectedUnit.Health / SelectedUnit.MaxHealth));
            }
        }
    }
}
