using System.Numerics;
using TMPro;
using UnityEngine;

namespace WebGlAuth.Example
{
    public class WebGlAuthDemo : MonoBehaviour
    {
        [SerializeField] TMP_InputField _addressText;
        [SerializeField] TMP_InputField _chainIdText;
        [SerializeField] TMP_InputField _errorText;

        private string _address;
        private BigInteger _chainId;
        private string _lastError;

        void Start()
        {
            MetamaskController.Instance.NewAccountSelectedEvent += OnNewAccountSelectedEvent;
            MetamaskController.Instance.ChainChangedEvent += OnChainChangedEvent;
            MetamaskController.Instance.ErrorEvent += OnErrorEvent;
            UpdateView();
        }

        private void UpdateView()
        {
            if (MetamaskController.Instance.IsMetamaskInitialised)
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
            _address = address;
            _lastError = string.Empty;
            UpdateView();
        }

        private void OnChainChangedEvent(BigInteger chainId)
        {
            _chainId = chainId;
            _lastError = string.Empty;
            UpdateView();
        }

        private void OnErrorEvent(string error)
        {
            _lastError = error;
            UpdateView();
        }
    }
}
