using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedKite
{
    public class CombatMenu : MonoBehaviour
    {
        public GameObject actionSelection;
        public GameObject skillTypeSelection;
        public GameObject skillSelection;
        public GameObject menu;

        PathFinder pathFinder = new PathFinder();

        List<GameSprite> sprites;

        public bool IsActive;
        public bool ActionTaken;

        bool canAttack;
        bool canAssist;

        int maxAttackRange;
        int maxAssistRange;

        List<Enemy> attackables;
        List<Prop> interactables;
        List<Hero> assistables;

        Reticle reticle;
        // Start is called before the first frame update
        void Start()
        {
            sprites = GameSpriteManager.Instance.Sprites;
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void Activate(Unit unit, Vector3Int selectedTile)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                ActionTaken = false;

                IsActive = true;

                if (menu == null)
                {
                    menu = Instantiate(actionSelection);
                }

                menu.transform.SetParent(transform);
                menu.transform.localScale = Vector3.one;
                Vector3 pos = Camera.main.WorldToScreenPoint(hit.point);
                pos = new Vector2(pos.x - Screen.width / 2, pos.y - Screen.height / 2);

                RectTransform rect = menu.GetComponent<RectTransform>();

                if (pos.x + rect.rect.xMax > Screen.width / 2)
                    pos.x = Screen.width / 2 - rect.sizeDelta.x;

                if (pos.y + rect.rect.yMax > Screen.height / 2)
                    pos.y = Screen.height / 2 - rect.sizeDelta.y;

                rect.localPosition = pos;

                GetMaxRanges(unit);
            }
            else
            {
                return;
            }
        }
        public void Deactivate()
        {
            Destroy(menu);
            IsActive = false;
        }

        public void GetMaxRanges(Unit unit)
        {

            foreach (Skill skill in unit.Skills)
            {
                if ((skill is Weapon & skill.Anti == false) | (skill is Buff & skill.Anti == false))
                    if (skill.Range > maxAttackRange)
                        maxAttackRange = skill.Range;
                    else
                    if (skill.Range > maxAttackRange)
                        maxAssistRange = skill.Range;
            }

        }

        public void FindInteractions(Unit unit, Vector3Int selectedTile)
        {
            foreach (GameSprite sprite in sprites)
            {
                if (sprite is Enemy & Utility.ManhattanDistance(selectedTile, sprite.Coordinate) <= maxAttackRange)
                {
                    Node[] range = GenerateBoxRange(selectedTile, Utility.ManhattanDistance(selectedTile, sprite.Coordinate));

                    bool result = pathFinder.IsReachable(unit, PathFinder.graph[sprite.Coordinate.x, sprite.Coordinate.y], range);

                    if (result)
                        attackables.Add((Enemy)sprite);
                }
                else if (sprite is Hero & Utility.ManhattanDistance(selectedTile, sprite.Coordinate) <= maxAssistRange)
                {
                    Node[] range = GenerateBoxRange(selectedTile, Utility.ManhattanDistance(selectedTile, sprite.Coordinate));

                    bool result = pathFinder.IsReachable(unit, PathFinder.graph[sprite.Coordinate.x, sprite.Coordinate.y], range);

                    if (result)
                        assistables.Add((Hero)sprite);
                }
                else if (sprite is Prop)
                { }
            }
        }

        Node[] GenerateBoxRange(Vector3Int startingSpot, int distance)
        {
            List<Node> range = new List<Node>();

            Vector2Int cell;

            for (int i = 0; i < distance; i++)
            {
                for (int j = 0; j < distance; j++)
                {
                    cell = new Vector2Int((int)startingSpot.x + i, (int)startingSpot.y + j);

                    if (cell.x >= 0 & cell.x < TileMapper.Instance.W & cell.y >= 0 & cell.y < TileMapper.Instance.H)
                    {
                        range.Add(PathFinder.graph[cell.x, cell.y]);
                    }
                }
            }

            return range.ToArray();
        }


    }
}