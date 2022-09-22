using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UnitManager : MonoBehaviour {
    public static UnitManager Instance;

    private List<ScriptableUnit> _units;
    public BaseHero SelectedHero;
    public BaseHero tankPlayer;
    [SerializeField][Range(0,1)]
    public float speedMove = 1f;

    public Slider slider;

    void Awake() {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();

    }

    public void SpawnHeroes() 
    {
        var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
        var robotFixedPos = GetFixedRotationRobot(StudentSolution.Instance.direction);
        var spawnedHero = Instantiate(randomPrefab,new Vector3(5f,7f,-1f), Quaternion.Euler(0,0,robotFixedPos));
        tankPlayer = spawnedHero;
        //for random position
        /*var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();
        randomSpawnTile.SetUnit(spawnedHero);*/
        var randomSpawnTile = GridManager.Instance.GetTileAtPosition(new Vector2(5f,7f) );
        randomSpawnTile.SetUnit(spawnedHero);
        
        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    public void SpawnEnemies()
    {
        var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
        var spawnedEnemy = Instantiate(randomPrefab,new Vector3(30,40,-1f), Quaternion.Euler(0,0,90));
        
        //for random position
        //var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();
        var randomSpawnTile = GridManager.Instance.GetTileAtPosition(new Vector2(30f,40f) );
        randomSpawnTile.SetUnit(spawnedEnemy);
        

        GameManager.Instance.ChangeState(GameState.StartMoving);
    }

    public IEnumerator StartMovingTank()
    {
        var tempMovesGraph = StudentSolution.Instance.movesGraph;
        for (var i = 0; i < tempMovesGraph.Count; i++)
        {
            if (Math.Abs(tempMovesGraph[i].x - (-5)) < 0.2f)//tempMovesGraph[i].x == -5)
            {
                tankPlayer.transform.rotation = Quaternion.Euler(0,0,GetFixedRotationRobot((int)tempMovesGraph[i].y));
                
            }
            else
            {
                var randomSpawnTile = GridManager.Instance.GetTileAtPosition(new Vector2(tempMovesGraph[i].x,tempMovesGraph[i].y) );
                randomSpawnTile.SetUnit(tankPlayer);
                //transform.position = new Vector3 (tempMovesGraph[i].x, tempMovesGraph[i].y, 0);
            }
            
            yield return new WaitForSeconds(speedMove); 
        }
        if(!StudentSolution.Instance.SolutionFound)
            MenuManager.Instance.ShowSelectedHero("No Solution Found", false);
        else
            MenuManager.Instance.ShowSelectedHero("Reached Goal", true);
        yield return null;
    }

    public void SetSpeedTank()
    {
        speedMove = slider.value;
    }
    
    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit {
        return (T)_units.Where(u => u.Faction == faction ).OrderBy(o => Random.value).First().UnitPrefab;
    }

    public int GetFixedRotationRobot(int x)
    {
        switch (x)
        {
            case 0:
                return 270;
            case 1:
                return 225;
            case 2:
                return 180;
            case 3:
                return 135;
            case 4:
                return 90;
            case 5:
                return 45;
            case 6:
                return 0;
            case 7:
                return 315;
            default:
                return 90;
        }
    }
}
