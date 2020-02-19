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

        public static bool IsActive { get; private set; }

        Reticle reticle;

        BattleGrid battleGrid;
        Grid grid;
        // Start is called before the first frame update
        void Start()
        {
            sprites = GameSpriteManager.Instance.Sprites;
            battleGrid = FindObjectOfType<BattleGrid>();
            grid = FindObjectOfType<Grid>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void ActivatePopUp(Unit unit, Vector3Int selectedTile)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
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

                Actionables actionables = FindInteractions(unit, selectedTile);

                menu.GetComponent<CombatMenuPopUp>().Activate(unit, selectedTile, actionables, battleGrid);
            }
        }

        public void Deactivate()
        {
            Destroy(menu);
            battleGrid.Clear();
            IsActive = false;
        }

        public Actionables FindInteractions(Unit unit, Vector3Int selectedTile)
        {

            List<Enemy> attackables = new List<Enemy>();
            List<Prop> interactables = new List<Prop>();
            List<Hero> assistables = new List<Hero>();

            Debug.Log("Highlight " + selectedTile);
            Debug.Log("Unit: " + unit.Coordinate);

            Node[] attackNodes = Utility.GenerateBoxRange(selectedTile, unit.MaxAttackRange);
            Node[] assistNodes = Utility.GenerateBoxRange(selectedTile, unit.MaxAssistRange);

            foreach (GameSprite sprite in sprites)
            {
                if (sprite is Enemy enemy)
                {
                    if (Utility.ManhattanDistance(new Vector3Int(selectedTile.x, selectedTile.y, -1), new Vector3Int(enemy.Coordinate.x, enemy.Coordinate.y, -1)) <= unit.MaxAttackRange)
                    {
                        bool result = pathFinder.IsReachable(PathFinder.graph[selectedTile.x, selectedTile.y], PathFinder.graph[enemy.Coordinate.x, enemy.Coordinate.y], attackNodes, unit.MaxAttackRange, false);

                        if (result)
                            attackables.Add(enemy);
                    }
                }
                else if (sprite is Hero hero)
                {
                    if (Utility.ManhattanDistance(new Vector3Int(selectedTile.x, selectedTile.y, -1), new Vector3Int(hero.Coordinate.x, hero.Coordinate.y, -1)) <= unit.MaxAssistRange | hero.Equals(unit))
                    {
                        bool result = false;

                        if(!hero.Equals(unit))
                            result = pathFinder.IsReachable(PathFinder.graph[selectedTile.x, selectedTile.y], PathFinder.graph[hero.Coordinate.x, hero.Coordinate.y], assistNodes, unit.MaxAssistRange, false);

                        if (result | hero.Equals(unit))
                            assistables.Add(hero);
                    }
                }
                else if (sprite is Prop prop)
                {
                    if (prop.IsInteractable & Utility.ManhattanDistance(selectedTile, prop.Coordinate) <= 1)
                    {
                        interactables.Add(prop);
                    }
                }
            }

            Actionables actionables = new Actionables(attackables, interactables, assistables);

            return actionables;
        }


        public struct Actionables
        {
            public List<Enemy> Attackables;
            public List<Prop> Interactables;
            public List<Hero> Assistables;

            public Actionables(List<Enemy> _attackables, List<Prop> _interactables, List<Hero> _assistables)
            {
                Attackables = _attackables;
                Interactables = _interactables;
                Assistables = _assistables;
            }
        }
    }
}