using WarGame.Models.Responses;
using WarGame.Models;

public interface IDeckService
{
    Player[] DealCards(); // Initializes the game
    PlayResponse Play(); // Plays a single round
}
