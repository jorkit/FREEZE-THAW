using FreezeThaw.Utils;
using Godot;
using System;
using static BigBro;

public partial class WaitingHall : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BigBro.CreatePlayerContainer();
        if (BigBro.IsMultiplayer == true)
        {
            /* Spawner must add first before playerContainer add and then set the SpawnPath */
            BigBro.Spawner = new();
            BigBro.bigBro.AddChild(BigBro.Spawner);
            foreach (var path in Character.CharacterPathList)
            {
                BigBro.Spawner.AddSpawnableScene(path.Value);
            }
            BigBro.bigBro.AddChild(BigBro.PlayerContainer);
            BigBro.Spawner.SpawnPath = BigBro.PlayerContainer.GetPath();
            if (BigBro.MultiplayerApi.IsServer() == true)
            {
                BigBro.PlayerAdd(((int)Character.CharacterTypeEnum.AISandworm).ToString(), Character.CharacterPathList[Character.CharacterTypeEnum.AISandworm]);
                BigBro.PlayerAdd(((int)Character.CharacterTypeEnum.AIMouse).ToString(), Character.CharacterPathList[Character.CharacterTypeEnum.AIMouse]);
            }
        }
        else
        {
            BigBro.bigBro.AddChild(BigBro.PlayerContainer);
            //BigBro.PlayerAdd(((int)Character.CharacterTypeEnum.AISandworm).ToString(), Character.CharacterPathList[Character.CharacterTypeEnum.AISandworm]);
            BigBro.PlayerAdd("1", Character.CharacterPathList[Character.CharacterTypeEnum.Sandworm]);
            //BigBro.PlayerAdd(((int)Character.CharacterTypeEnum.Mouse).ToString(), Character.CharacterPathList[Character.CharacterTypeEnum.Mouse]);
            BigBro.PlayerAdd(((int)Character.CharacterTypeEnum.AIMouse).ToString(), Character.CharacterPathList[Character.CharacterTypeEnum.AIMouse]);
        }
    }
}
