using Core;
using Logger = Core.Logger;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
   [SerializeField] 
   private Button startServerButton;
   
   [SerializeField] 
   private Button startHostButton;
   
   [SerializeField] 
   private Button startClientButton;
   
   [SerializeField] 
   private TextMeshProUGUI playersInGameText;
   
   [SerializeField] 
   private Button executePhysicsButton;

   private bool hasServerStarted;

   private void Awake()
   {
      Cursor.visible = true;
   }

   private void Update()
   {
      playersInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
   }

   private void Start()
   {
      startHostButton.onClick.AddListener(() =>
      {
         if (NetworkManager.Singleton.StartHost())
         {
            Logger.Instance.LogInfo("Host started...");
         }
         else
         {
            Logger.Instance.LogError("Host could not be started...");
         }
      });
      
      startServerButton.onClick.AddListener(() =>
      {
         if (NetworkManager.Singleton.StartServer())
         {
            Logger.Instance.LogInfo("Server started...");
         }
         else
         {
            Logger.Instance.LogError("Server could not be started...");
         }
      });
      
      startClientButton.onClick.AddListener(() =>
      {
         if (NetworkManager.Singleton.StartClient())
         {
            Logger.Instance.LogInfo("Client started...");
         }
         else
         {
            Logger.Instance.LogError("Client could not be started...");
         }
      });
      
      NetworkManager.Singleton.OnServerStarted += () =>
      {
            NetworkObjectPool.Instance.InitializePool();
      };
      
      executePhysicsButton.onClick.AddListener(() =>
      {
         if (!hasServerStarted)
         {
            Logger.Instance.LogWarning("Server is not started...");
         }
         
         SpawnerControl.Instance.SpawnObjects();
      });
   }
}
