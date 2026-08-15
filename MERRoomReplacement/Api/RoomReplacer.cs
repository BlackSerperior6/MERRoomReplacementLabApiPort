using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using MapGeneration;
using MEC;
using MERRoomReplacement.Api.Structures;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace MERRoomReplacement.Api;

public static class RoomReplacer
{
    private static readonly IDictionary<RoomName, CachedRoom> RoomsTransformDataCache;

    static RoomReplacer()
    {
        RoomsTransformDataCache = new Dictionary<RoomName, CachedRoom>();
    }

    /// <summary>
    ///     Replaces room with MapEditorReborn schematic
    /// </summary>
    /// <param name="roomType">Room that should be replaced</param>
    /// <param name="roomSchematic">Replacement options</param>
    /// <returns>
    ///     <see cref="SchematicObject" />
    /// </returns>
    public static SchematicObject ReplaceRoom(RoomName roomType, RoomSchematic roomSchematic)
    {
        var room = Room.Get(roomType).FirstOrDefault();

        var schematicPosition = room.Position + roomSchematic.PositionOffset.ToUnityEngineVector();
        var schematicRotation = roomSchematic.RotationOffset.ToUnityEngineVector();

        if (room == null)
        {
            return TryReplaceCachedRoom(roomType, roomSchematic, schematicPosition, schematicRotation,
                out var cachedRoomData)
                ? null
                : cachedRoomData.Schematic;
        }
        
        DestroyRoom(room);

        var schematic = ObjectSpawner.SpawnSchematic(roomSchematic.SchematicName, schematicPosition, 
            Quaternion.Euler(schematicRotation + room.Transform.localRotation.eulerAngles));
        
        LabApi.Features.Console.Logger.Debug($"[{roomType}->{roomSchematic.SchematicName}] Schematic spawned at {schematic.Position}");

        var roomDetails = new CachedRoom(room.Position, room.Rotation.eulerAngles, schematic);

        if (RoomsTransformDataCache.ContainsKey(roomType))
        {
            RoomsTransformDataCache[roomType] = roomDetails;
            return schematic;
        }

        RoomsTransformDataCache.Add(roomType, roomDetails);

        return schematic;
    }

    /// <summary>
    ///     Replaces room with MapEditorReborn schematic
    /// </summary>
    /// <param name="roomType">Room that should be replaced</param>
    /// <param name="roomSchematic">Replacement options</param>
    /// <param name="delay">Delay in seconds until replacement</param>
    public static void ReplaceRoom(RoomName roomType, RoomSchematic roomSchematic, float delay)
    {
        LabApi.Features.Console.Logger.Debug($"[{roomType}->{roomSchematic.SchematicName}] Starting replacement coroutine with {delay} seconds delay");
        
        Timing.CallDelayed(delay, () =>
        {
            
            LabApi.Features.Console.Logger.Debug($"[{roomType}->{roomSchematic.SchematicName}] Coroutine: Replacing...");
            
            _ = ReplaceRoom(roomType, roomSchematic);
        });
    }
    
    /// <summary>
    ///     Destroys the specified room
    /// </summary>
    /// <remarks>
    ///     This method will not destroy the 079 components (cameras, speakers) in room
    /// </remarks>
    /// <param name="room">Room that should be destroyed</param>
    public static void DestroyRoom(Room room)
    {
        foreach (var component in room.GameObject.GetComponentsInChildren<Component>())
            try
            {

                LabApi.Features.Console.Logger.Debug($"Destroying component: {component.name} {component.tag} {component.GetType().FullName}");

                Object.Destroy(component);
            }
            catch
            {
                // ignored
            }
    }

    private static bool TryReplaceCachedRoom(RoomName roomType, RoomSchematic roomSchematic, Vector3 schematicPosition,
        Vector3 schematicRotation, out CachedRoom cachedRoomData)
    {
        if (!RoomsTransformDataCache.TryGetValue(roomType, out cachedRoomData))
            return true;

        schematicPosition += cachedRoomData.Position;
        schematicRotation += cachedRoomData.Rotation;

        cachedRoomData.Schematic.Destroy();
        
        cachedRoomData.Schematic = ObjectSpawner.SpawnSchematic(roomSchematic.SchematicName,
            schematicPosition, Quaternion.Euler(schematicRotation));

        return false;
    }
}