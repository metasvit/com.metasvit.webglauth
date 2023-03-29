mergeInto(LibraryManager.library, {
    EnableEthereum: async function (gameObjectName, callback, fallback) {
        dispatchReactUnityEvent("EnableEthereum", UTF8ToString(gameObjectName), UTF8ToString(callback), UTF8ToString(fallback));
    },
    EthereumInit: function(gameObjectName, callBackAccountChange, callBackChainChange){
        dispatchReactUnityEvent("EthereumInit", UTF8ToString(gameObjectName), UTF8ToString(callBackAccountChange), UTF8ToString(callBackChainChange));
    },
    GetChainId: async function(gameObjectName, callback, fallback) {
        dispatchReactUnityEvent("GetChainId", UTF8ToString(gameObjectName), UTF8ToString(callback), UTF8ToString(fallback));
    },
    IsMetamaskAvailable: function () {
        if (window.ethereum) return true;
        return false;
    },
    Request: async function (message, gameObjectName, callback, fallback ) {
        dispatchReactUnityEvent("Request", UTF8ToString(message), UTF8ToString(gameObjectName), UTF8ToString(callback), UTF8ToString(fallback));
    },
     EthereumInitRpcClientCallback: function(callBackAccountChange, callBackChainChange) {   
        ethereum.on("accountsChanged",
                function (accounts) {
                    let account = "";
                    if(accounts[0] !== undefined){
                        account = accounts[0];
                    }
                    var len = lengthBytesUTF8(account) + 1;
                    var strPtr = _malloc(len);
                    stringToUTF8(account, strPtr, len);
                    Module.dynCall_vi(callBackAccountChange, strPtr);
                });
        ethereum.on("chainChanged",
                function (chainId) {
                    var len = lengthBytesUTF8(chainId.toString()) + 1;
                    var strPtr = _malloc(len);
                    stringToUTF8(chainId.toString(), strPtr, len);
                    Module.dynCall_vi(callBackChainChange, strPtr);
                });
    },
    RequestRpcClientCallback: async function (callback, message) {
        const parsedMessageStr = UTF8ToString(message);
        const parsedCallback = UTF8ToString(callback);
      
        //console.log(parsedCallback);
        let parsedMessage = JSON.parse(parsedMessageStr);
        try {
            
            //console.log(parsedMessage);
            const response = await ethereum.request(parsedMessage);
            let rpcResponse = {
                jsonrpc: "2.0",
                result: response,
                id: parsedMessage.id,
                error: null
            }
            //console.log(rpcResponse);

            var json = JSON.stringify(rpcResponse);
            //console.log(json);
           
            var len = lengthBytesUTF8(json) + 1;
            var strPtr = _malloc(len);
            stringToUTF8(json, strPtr, len);
            Module.dynCall_vi(callback, strPtr);

        } catch (e) {
            //console.log(e);
            let rpcResonseError = {
                jsonrpc: "2.0",
                id: parsedMessage.id,
                error: {
                    message: e.message,
                }
            }
            var json = JSON.stringify(rpcResonseError);
            //console.log(json);
            var len = lengthBytesUTF8(json) + 1;
            var strPtr = _malloc(len);
            stringToUTF8(json, strPtr, len);

            Module.dynCall_vi(callback, strPtr);
        }
    }
});