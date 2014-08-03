using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tresors : MonoBehaviour
{
    private const string LOCKEY_TRESOR_PLACEMENT_MSG = "placement_msg";
    private const string LOCKEY_TRESOR_PLACEMENT_REMAINING = "placement_remain";

    private static Tresors _instance;
    public static Tresors Instance { get { return _instance; } }

    public Transform tresorsParent;
    public GameObject[] tresorPrefabs;
    public GameObject tresorUiPanel;
    public UILabel tresorUiRemainingLabel;
    public GameObject tresorUiDoneButton;

    public int nbTresorsPerPlayer;
    public float tresorPositionZ;

    private int currentPlayerId;
    private int currentNbTresorsForPlayer;
    private int currentTresorId;

	void Awake()
    {
        _instance = this;
        currentTresorId = 0;
    }

    public void startTresorPlacementForPlayer(int playerId)
    {
        currentPlayerId = playerId;
        tresorUiPanel.SetActive(false);
        PopupManager.Instance.showPopupWithMessage(new LocalizedMessage(LOCKEY_TRESOR_PLACEMENT_MSG, playerId + 1, nbTresorsPerPlayer), onReadyForTresorPlacement);
    }

    public void onReadyForTresorPlacement()
    {
        tresorUiPanel.SetActive(true);
        setCurrentNbTresorsForPlayer(nbTresorsPerPlayer);
    }

    private void setCurrentNbTresorsForPlayer(int value)
    {
        currentNbTresorsForPlayer = value;
        LocalizationUtils.FillLabelWithLocalizationKey(tresorUiRemainingLabel, LOCKEY_TRESOR_PLACEMENT_REMAINING, currentNbTresorsForPlayer);
        bool tresorsLeft = currentNbTresorsForPlayer > 0;
        tresorUiRemainingLabel.gameObject.SetActive(tresorsLeft);
        tresorUiDoneButton.SetActive(!tresorsLeft);
    }

    public void placeTresorInCell(BoardCell cell)
    {
        if ((cell.treasure == null) && (currentNbTresorsForPlayer != 0))
        {
            Vector3 tresorPosition = cell.transform.position;
            tresorPosition.z = tresorPositionZ;
            GameObject tresorPrefab = tresorPrefabs[currentTresorId];
            GameObject tresorObject = Instantiate(tresorPrefab, tresorPosition, Quaternion.identity) as GameObject;
            tresorObject.transform.localScale = cell.transform.localScale;
            tresorObject.transform.parent = tresorsParent;
            cell.treasure = tresorObject;
            cell.treasurePlayerId = currentPlayerId;
            setCurrentNbTresorsForPlayer(currentNbTresorsForPlayer-1);
        }
        else if ((cell.treasure != null) && (cell.treasurePlayerId == currentPlayerId))
        {
            DestroyImmediate(cell.treasure.gameObject);
            cell.treasure = null;
            cell.treasurePlayerId = -1;
            setCurrentNbTresorsForPlayer(currentNbTresorsForPlayer+1);
        }
    }

    public void onTresorPlacementDone()
    {
        tresorUiPanel.SetActive(false);
        currentTresorId = (currentTresorId + 1) % tresorPrefabs.Length;
        PlayersManager.Instance.onPlacementTresorDoneForPlayer();
    }
}
