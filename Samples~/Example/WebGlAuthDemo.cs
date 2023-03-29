using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WebGlAuth.Example
{
    public class WebGlAuthDemo : MonoBehaviour
    {
        [SerializeField] TMP_InputField _addressText;
        [SerializeField] TMP_InputField _chainIdText;
        [SerializeField] TMP_InputField _errorText;
        [SerializeField] Button _tryEnableEthereumButton;

        private string _address;
        private BigInteger _chainId;
        private string _lastError;

        void Start()
        {
            Debug.Log("WebGlAuthDemo.Start()");
            _tryEnableEthereumButton.onClick.AddListener(TryEnableEthereum);
            MetamaskController.NewAccountSelectedEvent += OnNewAccountSelectedEvent;
            MetamaskController.ChainChangedEvent += OnChainChangedEvent;
            MetamaskController.ErrorEvent += OnErrorEvent;
            if (!MetamaskController.IsMetamaskInitialised)
            {
                MetamaskController.TryEnableEthereum();
            }
            else
            {
                _address = MetamaskController.SelectedAccountAddress;
                _chainId = MetamaskController.CurrentChainId;
            }

            UpdateView();
        }

        void OnDestroy()
        {
            Debug.Log("WebGlAuthDemo.OnDestroy()");
            _tryEnableEthereumButton.onClick.RemoveAllListeners();
            MetamaskController.NewAccountSelectedEvent -= OnNewAccountSelectedEvent;
            MetamaskController.ChainChangedEvent -= OnChainChangedEvent;
            MetamaskController.ErrorEvent -= OnErrorEvent;
        }

        private void TryEnableEthereum()
        {
            Debug.Log("WebGlAuthDemo.TryEnableEthereum()");
            MetamaskController.TryEnableEthereum();
        }

        private void UpdateView()
        {
            Debug.Log($"WebGlAuthDemo.UpdateView(): MetamaskController.IsMetamaskInitialised = {MetamaskController.IsMetamaskInitialised}");
            if (MetamaskController.IsMetamaskInitialised)
            {
                _addressText.text = _address;
                _chainIdText.text = _chainId.ToString();
                _errorText.text = _lastError;
            }
            else
            {
                _addressText.text = string.Empty;
                _chainIdText.text = string.Empty;
            }

            _errorText.text = _lastError;
        }

        private void OnNewAccountSelectedEvent(string address)
        {
            Debug.Log($"WebGlAuthDemo.OnNewAccountSelectedEvent({address})");
            _address = address;
            _lastError = string.Empty;
            UpdateView();
        }

        private void OnChainChangedEvent(BigInteger chainId)
        {
            Debug.Log($"WebGlAuthDemo.OnChainChangedEvent({chainId})");
            _chainId = chainId;
            _lastError = string.Empty;
            UpdateView();
        }

        private void OnErrorEvent(string error)
        {
            Debug.Log($"WebGlAuthDemo.OnErrorEvent({error})");
            _lastError = error;
            UpdateView();
        }
    }
}
