using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RedKite
{ 
    public class FlashDisplay : MonoBehaviour
    {
        Text text;
        public bool Active { get; private set; } = false;
        bool completedWriting;
        float fadeSpeed = 0.02f;
        float fadeAmount = 0;
        float timeComplete = 0;
        float staticTime = 5f;
        FlashMessage flashMessage;
        string message;
        float messageSpeed = 0.02f;
        float timeSinceLastLetter = 0;
        int messageIndex = 0;
        // Start is called before the first frame update
        void Start()
        {
            text = GetComponentInChildren<Text>();
        }

        public void DisplayMessage(FlashMessage message)
        {
            text.color = message.color;

            text.text = "";

            flashMessage = message;

            Active = true;

            completedWriting = false;

            text.enabled = true;
        }

        // Update is called once per frame
        void Update()
        {
             if(Active)
            {
                if(!completedWriting)
                {
                    if(timeSinceLastLetter >= messageSpeed & messageIndex < flashMessage.message.Length)
                    {
                        timeSinceLastLetter = 0;

                        message += flashMessage.message[messageIndex];

                        text.text = message;

                        messageIndex++;
                    }
                    if (messageIndex >= flashMessage.message.Length)
                        completedWriting = true;

                    timeSinceLastLetter += Time.deltaTime;
                }
                else
                {
                    if(staticTime <= timeComplete)
                    { 
                        if(fadeAmount < 1)
                        {
                            fadeAmount += fadeSpeed;
                            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - fadeAmount);
                        }
                        else
                        {
                            Deactivate();
                        }
                    }
                    timeComplete += Time.deltaTime;
                }
            }
        }
        public void Deactivate()
        {
            Active = false;

            timeComplete = 0;
            staticTime = 0;
            messageIndex = 0;
            text.enabled = false;
            message = "";
            completedWriting = false;
            flashMessage = null;
            fadeAmount = 0;
        }
    }
}