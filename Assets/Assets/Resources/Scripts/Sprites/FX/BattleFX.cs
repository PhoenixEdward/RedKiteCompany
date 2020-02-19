using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace RedKite
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BattleFX : MonoBehaviour
    {
        SpriteRenderer sr;
        Sprite[] sprites;
        int spriteIndex = 0;
        readonly float secondsPerFrame = .125f;
        float timeSinceLastFrame;
        public static bool IsActive { get; private set; } = false;
        static Grid grid;
        Vector3Int coordinate;
        Text text;

        public enum Type
        {
            Melee,
            Ranged,
            Magic
        }
        // Start is called before the first frame update
        public void Activate(Type type, int _text, Vector3Int _coordinate)
        {
            IsActive = true;

            grid = grid ?? FindObjectOfType<Grid>();

            coordinate = _coordinate;

            sr = GetComponent<SpriteRenderer>();

            sr.sortingLayerName = "BattleFX";

            if (type == Type.Melee)
                sprites = Resources.LoadAll<Sprite>("BattleFX/Melee");
            else if (type == Type.Ranged)
                sprites = Resources.LoadAll<Sprite>("BattleFX/Ranged");
            else
                sprites = Resources.LoadAll<Sprite>("BattleFX/Magic");

            GameObject go = new GameObject();

            go.transform.SetParent(transform);
            go.transform.localPosition += new Vector3(0, 1, 0);

            GameObject[] textSprites;
            string digits = Mathf.Abs(_text).ToString();

            bool isDamage = _text < 0 ? true : false;

            if (digits.Length == 2)
            {
                textSprites = new GameObject[2];

                textSprites[0] = new GameObject();
                textSprites[1] = new GameObject();

                textSprites[0].AddComponent<TextFX>().Activate(Convert.ToInt32(digits[0] - 48), isDamage);
                textSprites[1].AddComponent<TextFX>().Activate(Convert.ToInt32(digits[1] - 48), isDamage);

                textSprites[0].transform.SetParent(go.transform);
                textSprites[0].transform.localPosition = new Vector3(-.05f, 0, 0);
                textSprites[1].transform.SetParent(go.transform);
                textSprites[1].transform.localPosition = new Vector3(-.05f, 0, 0);
            }
            else if(digits.Length == 3)
            {
                textSprites = new GameObject[3];

                textSprites[0] = new GameObject();
                textSprites[1] = new GameObject();
                textSprites[2] = new GameObject();

                textSprites[0].AddComponent<TextFX>().Activate(Convert.ToInt32(digits[0] - 48), isDamage);
                textSprites[1].AddComponent<TextFX>().Activate(Convert.ToInt32(digits[1] - 48), isDamage);
                textSprites[2].AddComponent<TextFX>().Activate(Convert.ToInt32(digits[2] - 48), isDamage);

                textSprites[0].transform.SetParent(go.transform);
                textSprites[0].transform.localPosition = new Vector3(-.075f, 0, 0);
                textSprites[1].transform.SetParent(go.transform);
                textSprites[1].transform.localPosition = new Vector3(0, 0, 0);
                textSprites[2].transform.SetParent(go.transform);
                textSprites[2].transform.localPosition = new Vector3(.075f, 0, 0);
            }
            else
            {
                textSprites = new GameObject[1];

                textSprites[0] = new GameObject();

                Debug.Log(digits[0]);

                textSprites[0].AddComponent<TextFX>().Activate(Convert.ToInt32(digits[0] - 48), isDamage);

                textSprites[0].transform.SetParent(go.transform);
                textSprites[0].transform.localPosition = Vector3.zero;
            }

        }

        // Update is called once per frame
        void Update()
        {
            Rotate();

            if (timeSinceLastFrame > secondsPerFrame & spriteIndex < sprites.Length - 1)
            {
                spriteIndex++;
                timeSinceLastFrame = 0;
            }
            else if (timeSinceLastFrame > secondsPerFrame & spriteIndex == sprites.Length - 1)
            {
                IsActive = false;
                Destroy(gameObject);
            }

            sr.sprite = sprites[spriteIndex];

            timeSinceLastFrame += Time.deltaTime;
        }

        void Rotate()
        {

            if (CameraMovement.facing == CameraMovement.Facing.NE)
            {
                transform.rotation = Quaternion.Euler(0, 45f, 0);

                transform.position = grid.CellToWorld(coordinate) + new Vector3(0, 0.5f, 0);

            }
            else if (CameraMovement.facing == CameraMovement.Facing.SE)
            {

                transform.rotation = Quaternion.Euler(0, 135f, 0);

                transform.position = grid.CellToWorld(coordinate) + new Vector3(0f, 0, 1f) + new Vector3(0, 0.5f, 0);
            }
            else if (CameraMovement.facing == CameraMovement.Facing.SW)
            {
                transform.rotation = Quaternion.Euler(0, 225f, 0);


                transform.position = grid.CellToWorld(coordinate) + new Vector3(1f, 0, 1f) + new Vector3(0, 0.5f, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 315f, 0);

                transform.position = grid.CellToWorld(coordinate) + new Vector3(1, 0, 0) + new Vector3(0, 0.5f, 0);
            }
        }
    }
}