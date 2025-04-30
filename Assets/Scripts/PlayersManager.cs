using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Unity.Netcode;
using UnityEngine;
using Logger = Core.Logger;

public class PlayersManager : Singleton<PlayersManager>
{
   private NetworkVariable<int> playersInGame = new NetworkVariable<int>();

   public int PlayersInGame
   {
      get
      {
         return playersInGame.Value;
      }
   }

   private void Start()
   {
      NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
      {
         if (IsServer)
         {
            Core.Logger.Instance.LogInfo($"{id} just connected");
            playersInGame.Value++;
         }
      };

      NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
      {
         if (IsServer)
         {
            Core.Logger.Instance.LogInfo($"{id} just disconnected");
            playersInGame.Value--;
         }
      };
   }
}

