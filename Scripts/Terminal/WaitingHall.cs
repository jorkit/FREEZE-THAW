using FreezeThaw.Utils;
using Godot;
using System;

public partial class WaitingHall : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        /* Spawner must add first before playerContainer add and then set the SpawnPath */
        BigBro.Spawner = new();
        BigBro.bigBro.AddChild(BigBro.Spawner);
        foreach (var path in BigBro.CharacterPathList)
        {
            BigBro.Spawner.AddSpawnableScene(path.Value);
        }

        BigBro.CreatePlayerContainer();
        BigBro.bigBro.AddChild(BigBro.PlayerContainer);
        BigBro.Spawner.SpawnPath = BigBro.PlayerContainer.GetPath();
        if (BigBro.MultiplayerApi.IsServer() == true)
        {
            BigBro.PlayerAdd(GetMultiplayerAuthority(), BigBro.CharacterPathList[BigBro.CharacterTypeEnum.Sandworm]);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
