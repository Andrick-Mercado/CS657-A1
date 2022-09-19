using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    public static MenuManager Instance;

    [SerializeField] private GameObject _selectedHeroObject,_tileObject,_tileUnitObject,_movesGameObject;

    void Awake() {
        Instance = this;
    }

    public void ShowTileInfo(Tile tile) {

        if (tile == null)
        {
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
            return;
        }

        _tileObject.GetComponentInChildren<Text>().text = tile.TileName;
        _tileObject.SetActive(true);

        if (tile.OccupiedUnit) {
            _tileUnitObject.GetComponentInChildren<Text>().text = tile.OccupiedUnit.UnitName;
            _tileUnitObject.SetActive(true);
        }
    }

    public void ShowSelectedHero(string t, bool displayMoves)
    {
        if (displayMoves)
        {
            _movesGameObject.GetComponentInChildren<Text>().text = $"Moves: {StudentSolution.Instance.moves}";
            _movesGameObject.SetActive(true);
        }
        _selectedHeroObject.GetComponentInChildren<Text>().text = t;
        _selectedHeroObject.SetActive(true);
        
    }
}
