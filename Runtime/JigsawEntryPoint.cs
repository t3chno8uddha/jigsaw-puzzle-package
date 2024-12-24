using System;
using System.Threading.Tasks;
using com.appidea.MiniGamePlatform.CommunicationAPI;
using UnityEngine;

public class JigsawEntryPoint : BaseMiniGameEntryPoint
{
    [SerializeField] private GameObject gamePrefab;
    protected override Task LoadInternal()
    {
        var gameManager = Instantiate(gamePrefab);
        gameManager.GetComponentInChildren<JigsawManager>().SetEntryPoint(this);
        return Task.CompletedTask;
    }

    protected override Task UnloadInternal()
    {
        return Task.CompletedTask;
    }
}