using System;
using System.Numerics;

namespace WebGlAuth
{
    public interface IMetamaskController
    {
        void TryEnableEthereum();

        bool IsMetamaskInitialised { get; }
        string SelectedAccountAddress { get; }
        BigInteger CurrentChainId { get; }

        public event Action<string> NewAccountSelectedEvent;
        public event Action<BigInteger> ChainChangedEvent;
        public event Action<string> ErrorEvent;
    }
}
