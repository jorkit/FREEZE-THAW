using FreezeThaw.Utils;
using Godot;
using System;
using static BigBro;

public partial class WaitingHall : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
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
                PlayerControler.PlayerAdd(GetMultiplayerAuthority().ToString(), Character.CharacterPathList[Character.CharacterTypeEnum.Sandworm], false);
                //PlayerControler.PlayerAdd(((int)Character.CharacterTypeEnum.Mouse).ToString(), Character.CharacterPathList[Character.CharacterTypeEnum.Mouse], false);
            }
        }
        else
        {
            //BigBro.PlayerAdd(((int)Character.CharacterTypeEnum.AISandworm).ToString(), Character.CharacterPathList[Character.CharacterTypeEnum.AISandworm]);
            PlayerControler.PlayerAdd("1", Character.CharacterPathList[Character.CharacterTypeEnum.Sandworm], false);
            //BigBro.PlayerAdd(((int)Character.CharacterTypeEnum.Mouse).ToString(), Character.CharacterPathList[Character.CharacterTypeEnum.Mouse]);
            PlayerControler.PlayerAdd(((int)Character.CharacterTypeEnum.Mouse).ToString(), Character.CharacterPathList[Character.CharacterTypeEnum.Mouse], true);
        }
    }
}
