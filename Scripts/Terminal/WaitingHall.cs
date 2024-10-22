using FreezeThaw.Utils;
using Godot;
using System;
using static BigBro;

public partial class WaitingHall : Node
{
    public static SubViewportContainer SelectingArea { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SelectingArea = GetNodeOrNull<SubViewportContainer>("SelectingArea");
        if (SelectingArea == null )
        {
            LogTool.DebugLogDump("SelectingArea not found!");
            return;
        }

        if (NetworkControler.IsMultiplayer == true)
        {
            /* Spawner must be added before playerContainer? and then set the SpawnPath */
            NetworkControler.Spawner = new();
            BigBro.NetworkControler.AddChild(NetworkControler.Spawner);
            foreach (var path in Character.CharacterPathList)
            {
                NetworkControler.Spawner.AddSpawnableScene(path.Value);
            }
            NetworkControler.Spawner.SpawnPath = PlayerControler.PlayerContainer.GetPath();
            if (NetworkControler.MultiplayerApi.IsServer() == true)
            {
                PlayerControler.PlayerAdd(GetMultiplayerAuthority().ToString(), Character.CharacterTypeEnum.Mouse, false);
            }
        }
        else
        {
            PlayerControler.PlayerAdd("1", Character.CharacterTypeEnum.Mouse, false);
            PlayerControler.PlayerAdd(((int)Character.CharacterTypeEnum.Mouse2).ToString(), Character.CharacterTypeEnum.Mouse, true);
            PlayerControler.PlayerAdd(((int)Character.CharacterTypeEnum.Mouse3).ToString(), Character.CharacterTypeEnum.Mouse, true);
            PlayerControler.PlayerAdd(((int)Character.CharacterTypeEnum.Mouse4).ToString(), Character.CharacterTypeEnum.Mouse, true);
            PlayerControler.PlayerAdd(((int)Character.CharacterTypeEnum.Mouse5).ToString(), Character.CharacterTypeEnum.Mouse, true);
        }
    }
}
