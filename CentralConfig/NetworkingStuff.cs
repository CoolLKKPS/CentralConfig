using LethalLevelLoader;
using System;
using System.Collections;
using System.Threading;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace CentralConfig
{
    public class ShareScrapValue : NetworkBehaviour
    {
        private static ShareScrapValue _instance;
        public static ShareScrapValue Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("ShareScrapValue").AddComponent<ShareScrapValue>();
                }
                return _instance;
            }
        }

        public float scrapValueMultiplier;

        public float CurrentMultiplier;

        public ManualResetEvent dataReceivedEvent = new ManualResetEvent(false);
        public void DetermineMultiplier(Action<float> callback)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                float multiplier = CalculateScrapValueMultiplier();
                callback(multiplier);
            }
            else
            {
                RequestData();
                StartCoroutine(WaitAndProcessData(callback));
            }
        }

        private IEnumerator WaitAndProcessData(Action<float> callback)
        {
            yield return new WaitUntil(() => dataReceivedEvent.WaitOne(0));
            callback(scrapValueMultiplier);
            dataReceivedEvent.Reset();
        }

        private void Awake()
        {
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(MessageName, OnMessageReceived);
        }

        [Tooltip("HostScrapValue")]
        public string MessageName = "Host Scrap Value Multiplier";

        public void RequestData()
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                var writer = new FastBufferWriter(0, Allocator.Temp);
                NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(MessageName, NetworkManager.ServerClientId, writer);
            }
        }

        public void UpdateAndSendData()
        {
            using (var writer = new FastBufferWriter(sizeof(float), Allocator.Temp))
            {
                writer.WriteValueSafe(scrapValueMultiplier);
                NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(MessageName, NetworkManager.Singleton.ConnectedClientsIds, writer);
            }
        }

        private void OnMessageReceived(ulong clientId, FastBufferReader reader)
        {
            if (NetworkManager.Singleton.IsHost && clientId != NetworkManager.ServerClientId)
            {
                scrapValueMultiplier = CalculateScrapValueMultiplier();
                // CentralConfig.instance.mls.LogInfo($"Host sending scrapValueMultiplier: {scrapValueMultiplier}");
                UpdateAndSendData();
            }
            else if (!NetworkManager.Singleton.IsHost)
            {
                reader.ReadValueSafe(out scrapValueMultiplier);
                // CentralConfig.instance.mls.LogInfo($"Client updated scrapValueMultiplier: {scrapValueMultiplier}");
                dataReceivedEvent.Set();
            }
        }

        /*private void Update()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                float newScrapValueMultiplier = CalculateScrapValueMultiplier();
                if (newScrapValueMultiplier != scrapValueMultiplier)
                {
                    scrapValueMultiplier = newScrapValueMultiplier;
                    UpdateAndSendData();
                }
            }
        }*/

        public float CalculateScrapValueMultiplier()
        {
            string weatherName = LevelManager.CurrentExtendedLevel.SelectableLevel.currentWeather.ToString();
            float scrapvaluemultiplier = 0.4f;
            if (CentralConfig.SyncConfig.DoScrapOverrides)
            {
                if (WaitForMoonsToRegister.CreateMoonConfig.ScrapValueMultiplier.TryGetValue(LevelManager.CurrentExtendedLevel, out var scrapValueMultiplierEntry))
                    scrapvaluemultiplier *= scrapValueMultiplierEntry;
            }
            if (CentralConfig.SyncConfig.DoScrapWeatherInjections)
            {
                if (WaitForWeathersToRegister.CreateWeatherConfig.WeatherScrapValueMultiplier.TryGetValue(weatherName, out var weatherScrapValueMultiplierEntry))
                    scrapvaluemultiplier *= weatherScrapValueMultiplierEntry;
            }
            else if (WRCompatibility.enabled)
            {
                scrapvaluemultiplier *= WRCompatibility.GetWRWeatherMultiplier();
            }
            if (CentralConfig.SyncConfig.ScaleScrapValueByPlayers)
            {
                float PlayerDiff = StartOfRound.Instance.connectedPlayersAmount + 1 - (float)MiscConfig.CreateMiscConfig.SSVBPThreshold;
                scrapvaluemultiplier *= Mathf.Clamp(Mathf.Pow(1f + ((MiscConfig.CreateMiscConfig.SSVBPPercentIncrease + (MiscConfig.CreateMiscConfig.SSVBPIncreaseChange * Mathf.Abs(PlayerDiff))) / 100f), -PlayerDiff), MiscConfig.CreateMiscConfig.SSVBPMinIncrease, MiscConfig.CreateMiscConfig.SSVBPMaxIncrease);
            }
            if (scrapvaluemultiplier < 0)
            {
                CentralConfig.instance.mls.LogWarning("Scrap Multiplier is negative, changing to 1x");
                scrapvaluemultiplier = 1f;
            }
            CentralConfig.instance.mls.LogInfo($"Scrap Multiplier: {scrapvaluemultiplier}");
            return scrapvaluemultiplier;
        }
    }
}