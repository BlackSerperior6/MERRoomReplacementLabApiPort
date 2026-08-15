using MapGeneration;
using MERRoomReplacement.Api.Structures;
using System.Collections.Generic;
using System.ComponentModel;

namespace MERRoomReplacement.Features.Configuration
{
    public class Config
    {
        [Description("Indicates plugin enabled or not")]
        public bool IsEnabled { get; set; } = true;

        [Description("Indicates debug mode enabled or not")]
        public bool Debug { get; set; } = false;

        [Description("Options for replacement")]
        public List<RoomSchematic> ReplacementOptions { get; set; } = new()
        {
            new RoomSchematic()
            {
                IsEnabled = false,
                TargetRoomType = RoomName.HczTesla,
                SchematicName = "AwesomeSchematic",
                SpawnChance = 50,
                SpawnDelay = 1f,
                PositionOffset = new Vector3(0, 0, 0),
                RotationOffset = new Vector3(0, 0, 0)
            }
        };
    }
}