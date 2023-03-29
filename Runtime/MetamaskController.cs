// #undef UNITY_EDITOR
using System;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.Unity.Metamask;
using UnityEngine;

namespace WebGlAuth
{
    public class MetamaskController : MonoBehaviour
    {
        public static bool IsMetamaskInitialised { get; private set; }
        public static string SelectedAccountAddress { get; private set; }
        public static BigInteger CurrentChainId { get; private set; }

        public static event Action<string> NewAccountSelectedEvent;
        public static event Action<BigInteger> ChainChangedEvent;
        public static event Action<string> ErrorEvent;

        private static MetamaskController _instance;

        [SerializeField] private bool _autoConnectOnStart = true;

        private void Start()
        {
            // Debug.Log("MetamaskController.Start()");
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            var instance = FindAnyObjectByType<MetamaskController>(FindObjectsInactive.Include);
            _instance = instance;
            DontDestroyOnLoad(gameObject);
            
            if (_autoConnectOnStart)
            {
                TryEnableEthereumInternal();
            }
        }

        public static void TryEnableEthereum()
        {
            if (_instance != null)
            {
                _instance.TryEnableEthereumInternal();
            }
        }

        private void TryEnableEthereumInternal()
        {
            // Debug.Log("MetamaskController.TryEnableEthereum()");
#if !UNITY_EDITOR
            if (MetamaskWebglInterop.IsMetamaskAvailable())
            {
                if (!IsMetamaskInitialised)
                {
                    // Debug.Log("MetamaskController.TryEnableEthereum() - EnableEthereum");
                    MetamaskWebglInterop.EnableEthereum(gameObject.name, nameof(EthereumEnabled), nameof(OnError));
                    return;
                }

                // Debug.Log("MetamaskController.TryEnableEthereum() - isMetamaskInitialised = true");
            }
            else
            {
                OnError("Metamask is not available, please install it");
            }
#endif
        }

        /// <summary>
        /// For internal use only!
        /// </summary>
        public void EthereumEnabled(string addressSelected)
        {
            // Debug.Log($"MetamaskController.EthereumEnabled({addressSelected})");
#if !UNITY_EDITOR
            if (!IsMetamaskInitialised)
            {
                // Debug.Log($"MetamaskController.EthereumEnabled() - EthereumInit");
                MetamaskWebglInterop.EthereumInit(gameObject.name, nameof(NewAccountSelected), nameof(ChainChanged));
                // Debug.Log($"MetamaskController.EthereumEnabled() IsMetamaskInitialised = true");
                IsMetamaskInitialised = true;
            }

            NewAccountSelected(addressSelected);
#endif
        }

        /// <summary>
        /// For internal use only!
        /// </summary>
        public void ChainChanged(string chainId)
        {
            // Debug.Log($"MetamaskController.ChainChanged({chainId})");
            CurrentChainId = new HexBigInteger(chainId).Value;
            ChainChangedEvent?.Invoke(CurrentChainId);
        }

        /// <summary>
        /// For internal use only!
        /// </summary>
        public void NewAccountSelected(string accountAddress)
        {
            // Debug.Log($"MetamaskController.NewAccountSelected({accountAddress})");
            SelectedAccountAddress = accountAddress;
            NewAccountSelectedEvent?.Invoke(SelectedAccountAddress);
#if !UNITY_EDITOR
            // Debug.Log($"MetamaskController.NewAccountSelected() - GetChainId");            
            MetamaskWebglInterop.GetChainId(gameObject.name, nameof(ChainChanged), nameof(OnError));
#endif
        }

        /// <summary>
        /// For internal use only!
        /// </summary>
        public void OnError(string errorMessage)
        {
            // Debug.Log($"MetamaskController.OnError({errorMessage})");
            ErrorEvent?.Invoke(errorMessage);
        }
    }
}
