#undef DEBUG
using System;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.Unity.Metamask;
using UnityEngine;

namespace WebGlAuth
{
    public class MetamaskController : MonoBehaviour, IMetamaskController
    {
        public static IMetamaskController Instance { get; private set; }

        [SerializeField] private bool _autoConnectOnStart = true;

        public bool IsMetamaskInitialised { get; private set; }

        public string SelectedAccountAddress { get; private set; }
        public BigInteger CurrentChainId { get; private set; }

        public event Action<string> NewAccountSelectedEvent;
        public event Action<BigInteger> ChainChangedEvent;
        public event Action<string> ErrorEvent;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            var instance = FindAnyObjectByType<MetamaskController>(FindObjectsInactive.Include);
            Instance = instance;
            DontDestroyOnLoad(gameObject);
            
            if (_autoConnectOnStart)
            {
                TryEnableEthereum();
            }
        }

        public void TryEnableEthereum()
        {
#if !UNITY_EDITOR
            if (MetamaskWebglInterop.IsMetamaskAvailable())
            {
                if (!IsMetamaskInitialised)
                {
                    MetamaskWebglInterop.EnableEthereum(gameObject.name, nameof(EthereumEnabled), nameof(OnError));
                }
            }
            else
            {
                OnError("Metamask is not available, please install it");
            }
#endif
        }

        private void EthereumEnabled(string addressSelected)
        {
#if !UNITY_EDITOR
            if (!IsMetamaskInitialised)
            {
                MetamaskWebglInterop.EthereumInit(gameObject.name, nameof(NewAccountSelected), nameof(ChainChanged));
                IsMetamaskInitialised = true;
                return;
            }

            NewAccountSelected(addressSelected);
#endif
        }

        private void ChainChanged(string chainId)
        {
            CurrentChainId = new HexBigInteger(chainId).Value;
            ChainChangedEvent?.Invoke(CurrentChainId);
        }

        private void NewAccountSelected(string accountAddress)
        {
            SelectedAccountAddress = accountAddress;
            NewAccountSelectedEvent?.Invoke(SelectedAccountAddress);
        }

        private void OnError(string errorMessage)
        {
            ErrorEvent?.Invoke(errorMessage);
        }
    }
}
