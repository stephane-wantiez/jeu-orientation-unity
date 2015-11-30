using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasuresManager : MonoBehaviour
{
    private const string LOCKEY_TRESOR_PLACEMENT_MSG = "placement_msg";
    private const string LOCKEY_TRESOR_PLACEMENT_REMAINING = "placement_remain";

    private static TreasuresManager _instance;
    public static TreasuresManager Instance { get { return _instance; } }

    public Transform treasureObjectsParent;
    public GameObject[] treasurePrefabs;
    public GameObject treasurePlacementUiPanel;
    public UILabel treasurePlacementUiRemainingLabel;
    public GameObject treasurePlacementUiDoneButton;
    public TreasuresUI treasuresGameUi;

    public int nbTreasuresPerPlayer;
    public float treasurePositionZ;

    [HideInInspector]
    public int nbTreasuresInGame;

    private int currentPlayerId;
    private int currentNbTreasuresForPlayer;
    private int currentTreasureId;

	void Awake()
    {
        _instance = this;
        currentTreasureId = 0;
    }

    void Start()
    {
        GameManager.Instance.OnGameStateChangeEvents += onGameStateChange;
    }

    public void startTresorPlacementForPlayer(int playerId)
    {
        currentPlayerId = playerId;
        treasurePlacementUiPanel.SetActive(false);
        PopupManager.Instance.showPopupWithMessage(new LocalizedMessage(LOCKEY_TRESOR_PLACEMENT_MSG, playerId + 1, nbTreasuresPerPlayer), onReadyForTresorPlacement);
    }

    public void onReadyForTresorPlacement()
    {
        treasurePlacementUiPanel.SetActive(true);
        setCurrentNbTresorsForPlayer(nbTreasuresPerPlayer);
    }

    private void setCurrentNbTresorsForPlayer(int value)
    {
        currentNbTreasuresForPlayer = value;
        LocalizationUtils.FillLabelWithLocalizationKey(treasurePlacementUiRemainingLabel, LOCKEY_TRESOR_PLACEMENT_REMAINING, currentNbTreasuresForPlayer);
        bool tresorsLeft = currentNbTreasuresForPlayer > 0;
        treasurePlacementUiRemainingLabel.gameObject.SetActive(tresorsLeft);
        treasurePlacementUiDoneButton.SetActive(!tresorsLeft);
    }

    public void placeTresorInCell(BoardCell cell)
    {
        if ((cell.treasure == null) && (currentNbTreasuresForPlayer != 0))
        {
            Vector3 tresorPosition = cell.transform.position;
            tresorPosition.z = treasurePositionZ;
            GameObject tresorPrefab = treasurePrefabs[currentTreasureId];
            GameObject tresorObject = Instantiate(tresorPrefab, tresorPosition, Quaternion.identity) as GameObject;
            tresorObject.transform.localScale = cell.transform.localScale;
            tresorObject.transform.parent = treasureObjectsParent;
            cell.treasure = tresorObject;
            cell.treasurePlayerId = currentPlayerId;
            setCurrentNbTresorsForPlayer(currentNbTreasuresForPlayer-1);
            ++nbTreasuresInGame;
        }
        else if ((cell.treasure != null) && (cell.treasurePlayerId == currentPlayerId))
        {
            DestroyImmediate(cell.treasure.gameObject);
            cell.treasure = null;
            cell.treasurePlayerId = -1;
            setCurrentNbTresorsForPlayer(currentNbTreasuresForPlayer+1);
            --nbTreasuresInGame;
        }
    }

    public void onTresorPlacementDone()
    {
        treasurePlacementUiPanel.SetActive(false);
        currentTreasureId = (currentTreasureId + 1) % treasurePrefabs.Length;
        PlayersManager.Instance.onPlacementTresorDoneForPlayer();
    }

    public void onTreasurePickup(GameObject treasure)
    {
        UnityEngine.Object.Destroy(treasure);
        --nbTreasuresInGame;
        treasuresGameUi.onTreasurePickup();
    }

    public bool hasTreasuresLeft()
    {
        return nbTreasuresInGame > 0;
    }

    private void onGameStateChange(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Game:
                treasuresGameUi.onGameStart();
                break;
        }
    }
}
