using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerNetworkManager : NetworkBehaviour {
    [SerializeField] private KitchenObjectListSo kitchenObjectListSo;

    public static MultiplayerNetworkManager Instance { get; private set; }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        Instance = this;
    }

    public void SpawnKitchenObjectByServer(KitchenObjectSo kitchenObjectSo, IKitchenObjectParent kitchenObjectParent) {
        var kitchenObjectSoIndex = GetKitchenObjectIndexInList(kitchenObjectSo);
        SpawnKitchenObjectServerRpc(kitchenObjectSoIndex, kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSoIndex,
        NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        var kitchenObjectSo = GetKitchenObjectSoByIndex(kitchenObjectSoIndex);
        var kitchenObjectTransform = Instantiate(kitchenObjectSo.prefab);

        var kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        kitchenObjectParentNetworkObjectReference.TryGet(out var kitchenObjectParentNetWorkObject);
        var kitchenObjectParent = kitchenObjectParentNetWorkObject.GetComponent<IKitchenObjectParent>();

        var kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    private int GetKitchenObjectIndexInList(KitchenObjectSo kitchenObjectSo) {
        return kitchenObjectListSo.kitchenObjectSos.IndexOf(kitchenObjectSo);
    }

    private KitchenObjectSo GetKitchenObjectSoByIndex(int kitchenObjectSoIndex) {
        return kitchenObjectListSo.kitchenObjectSos[kitchenObjectSoIndex];
    }

    public void DespawnKitchenObjectByServer(KitchenObject kitchenObject) {
        DespawnKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
        kitchenObjectNetworkObjectReference.TryGet(out var kitchenObjectNetworkObject);
        var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        
        DespawnKitchenObjectClientRpc(kitchenObjectNetworkObjectReference);
        
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void DespawnKitchenObjectClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
        kitchenObjectNetworkObjectReference.TryGet(out var kitchenObjectNetworkObject);
        var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        
        kitchenObject.ClearKitchenObjectForParent();
    }
}