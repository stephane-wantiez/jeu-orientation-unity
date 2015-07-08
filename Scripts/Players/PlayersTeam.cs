﻿using UnityEngine;

public class PlayersTeam
{
    public int teamId;
    public Player[] players;
    public int activePieceIndex;

    public void onPlayerActive(Player player)
    {
        activePieceIndex = -1;

        for (int i = 0; i < players.Length; ++i)
        {
            if (players[i] == player)
            {
                activePieceIndex = i;
                player.piece.setAsPlaying(true);
            }
            else
            {
                players[i].piece.setAsPlaying(false);
            }
        }
    }

    public void changePiece()
    {
        if (players.Length == 0) return;
        if (activePieceIndex >= 0) players[activePieceIndex].piece.setAsPlaying(false);
        activePieceIndex = (activePieceIndex + 1) % players.Length;
        players[activePieceIndex].piece.setAsPlaying(true);
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
