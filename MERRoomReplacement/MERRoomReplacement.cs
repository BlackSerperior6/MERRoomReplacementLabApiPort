using System;
using System.Linq;
using LabApi.Loader.Features.Plugins;
using MERRoomReplacement.Events.Handlers;
using MERRoomReplacement.Events.Interfaces;
using MERRoomReplacement.Features.Configuration;

namespace MERRoomReplacement
{
    public class MERRoomReplacement : Plugin<Config>
    {
        private IEventHandler _replacementHandler;

            
        public override string Name => "MERRoomReplacement";
        
        public override string Author => "FakeMan. Ported by BlackSerperior6";

        public override Version Version => new(1, 3, 1);

        public override string Description => "LabAPI port of MERRoomReplacement";

        public override Version RequiredApiVersion => LabApi.Features.LabApiProperties.CurrentVersion;

        public override void Enable()
        {
            _replacementHandler = new ReplacementHandler(
                Config.ReplacementOptions.Where(x => x.IsEnabled));

            _replacementHandler.SubscribeEvents();
        }

        public override void Disable()
        {
            _replacementHandler?.UnsubscribeEvents();
        }
    }
}