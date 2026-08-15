using System.Collections.Generic;
using LabApi.Events.Handlers;
using MERRoomReplacement.Api;
using MERRoomReplacement.Api.Structures;
using MERRoomReplacement.Events.Interfaces;
using UnityEngine;

namespace MERRoomReplacement.Events.Handlers;

public class ReplacementHandler : IEventHandler
{
    private readonly IEnumerable<RoomSchematic> _replacementOptions;
    
    public ReplacementHandler(IEnumerable<RoomSchematic> replacementOptions)
    {
        _replacementOptions = replacementOptions;
    }

    public void SubscribeEvents()
    {
        ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
    }

    public void UnsubscribeEvents()
    {
        ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;
    }
    private void OnWaitingForPlayers()
    {
        foreach (var roomSchematic in _replacementOptions)
        {
            LabApi.Features.Console.Logger.Debug($"Schematic name: {roomSchematic.SchematicName} | Room: {roomSchematic.TargetRoomType} | Chance: {roomSchematic.SpawnChance}%");

            if (roomSchematic.SpawnChance >= 100)
            {
                LabApi.Features.Console.Logger.Debug("Spawn chance is more or equal to 100, starting replacing");
            }
            else if (Random.Range(0, 101) is var chance && roomSchematic.SpawnChance >= chance)
            {
                LabApi.Features.Console.Logger.Debug($"Schematic chance {roomSchematic.SpawnChance} >= generated {chance}, starting replacing");
            }
            else
            {
                LabApi.Features.Console.Logger.Debug($"Generated chance {chance} is less then {roomSchematic}, skipping..");
                continue;
            }

            RoomReplacer.ReplaceRoom(roomSchematic.TargetRoomType, roomSchematic, roomSchematic.SpawnDelay);
        }
    }
}