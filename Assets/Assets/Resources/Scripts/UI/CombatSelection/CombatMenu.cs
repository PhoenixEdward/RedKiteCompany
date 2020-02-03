﻿using System.Collections;
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

        public bool IsActive { get; private set; }

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
            IsActive = false;
        }

        public Actionables FindInteractions(Unit unit, Vector3Int selectedTile)
        {

            int maxAttackRange = 0;
            int maxAssistRange = 0;

            //find the maximum distance for interactions
            foreach (Skill skill in unit.Weapons)
            {
                if (skill.Range > maxAttackRange)
                    maxAttackRange = skill.Range;
            }

            foreach (Skill skill in unit.Heals)
            {
                if (skill.Range > maxAttackRange)
                    maxAssistRange = skill.Range;
            }

            foreach (Skill skill in unit.Buffs)
            {
                if (skill.Range > maxAttackRange)
                    maxAssistRange = skill.Range;
            }

            foreach (Skill skill in unit.Debuffs)
            {
                if (skill.Range > maxAttackRange)
                    maxAttackRange = skill.Range;
            }

            List<Enemy> attackables = new List<Enemy>();
            List<Prop> interactables = new List<Prop>();
            List<Hero> assistables = new List<Hero>();

            Node[] attackNodes = GenerateBoxRange(selectedTile, maxAttackRange);
            Node[] assistNodes = GenerateBoxRange(selectedTile, maxAssistRange);

            foreach (GameSprite sprite in sprites)
            {
                if (sprite is Enemy enemy)
                {
                    if(Utility.ManhattanDistance(selectedTile, sprite.Coordinate) <= maxAttackRange)
                    { 

                        //bool result = pathFinder.IsReachable(unit, PathFinder.graph[sprite.Coordinate.x, enemy.Coordinate.y], attackNodes);

                        //if (result)
                            attackables.Add(enemy);
                    }
                }
                else if (sprite is Hero hero)
                {
                    if(Utility.ManhattanDistance(selectedTile, sprite.Coordinate) <= maxAssistRange)
                    { 

                        //bool result = pathFinder.IsReachable(unit, PathFinder.graph[sprite.Coordinate.x, sprite.Coordinate.y], assistNodes);

                        //if (result)
                            assistables.Add(hero);
                    }
                }
                else if (sprite is Prop prop)
                {
                    if (prop.IsInteractable & Utility.ManhattanDistance(selectedTile, sprite.Coordinate) <= 1)
                    {
                        interactables.Add(prop);
                    }
                }
            }

            Actionables actionables = new Actionables(attackables, interactables, assistables);

            return actionables;
        }

        Node[] GenerateBoxRange(Vector3Int _startingSpot, int _distance)
        {
            List<Node> range = new List<Node>();

            Vector2 startingSpot = new Vector2(_startingSpot.x - _distance, _startingSpot.y - _distance);

            int distance = _distance * 2;

            Vector2Int cell;

            for (int i = 0; i <= distance; i++)
            {
                for (int j = 0; j <= distance; j++)
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