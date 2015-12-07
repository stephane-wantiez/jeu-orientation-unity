using UnityEngine;

public class PlayersTeam
{
    public int teamId;
    public Player[] players;
    public int activePieceIndex;
    public int nbTreasures;

    public string getTeamIdAsStr()
    {
        return "" + (teamId + 1);
    }

    public void onPlayerActive(Player player)
    {
        activePieceIndex = -1;

        for (int i = 0; i < players.Length; ++i)
        {
            if (players[i] == player)
            {
                activePieceIndex = i;
            }
        }
    }

    public void resetTeam()
    {
        activePieceIndex = 0;
        nbTreasures = 0;
    }

    public void changeActivePlayerPiece()
    {
        if (players.Length == 0) return;
        activePieceIndex = (activePieceIndex + 1) % players.Length;
    }

    public PlayerPiece getActivePiece()
    {
        if ((players.Length == 0) || (activePieceIndex < 0) || (activePieceIndex >= players.Length)) return null;
        return players[activePieceIndex].piece;
    }

    public bool canChangePiece()
    {
        return (players.Length > 1);
    }
}
