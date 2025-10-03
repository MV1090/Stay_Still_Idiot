using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;


public class Prop : PlayerRole
{
    [SerializeField] private PropCatalog _catalog;    // assign in Inspector
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private LayerMask _propLayerMask;
    [SerializeField] private float _detectionRange = 10f;

    private GameObject _mainCamera;
        
    [SerializeField] private List<int> unlocked = new List<int>();

    private bool _propSelectorActive;

    private NetworkVariable<int> _currentCatalogIndex = new NetworkVariable<int>(
       -1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            _currentCatalogIndex.OnValueChanged += (_, idx) => ApplyDisguiseFromCatalog(idx);
            ApplyDisguiseFromCatalog(_currentCatalogIndex.Value);

            if (_mainCamera == null)
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

            if (!_meshFilter)
                _meshFilter = GetComponent<MeshFilter>();
        }
    }

    private void ApplyDisguiseFromCatalog(int idx)
    {
        if (_catalog == null || idx < 0 || idx >= _catalog.entries.Count) return;
            var mesh = _catalog.entries[idx].mesh;
        if (mesh != null && _meshFilter != null)
         _meshFilter.mesh = mesh;
    }

    [ServerRpc]
    private void SetDisguiseServerRpc(int catalogIndex)
    {
        if (catalogIndex < 0 || catalogIndex >= _catalog.entries.Count) return;
        _currentCatalogIndex.Value = catalogIndex;
    }

    private void TryPropSelect()
    {
        if(_mainCamera == null) return;

        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
        if (Physics.Raycast(ray, out var hit, _detectionRange, _propLayerMask))
        {
            PropObject newProp = hit.collider.GetComponent<PropObject>();

            if (newProp != null)
            {
                int catalogIndex = FindCatalogIndexById(newProp.PropObjectID);
                if (catalogIndex < 0) return;

                if(!unlocked.Contains(catalogIndex))
                    unlocked.Add(catalogIndex);

                SetDisguiseServerRpc(catalogIndex);
            }
        }
    }

    private int FindCatalogIndexById(string id)
    {
        if (_catalog == null) return -1;
        for (int i = 0; i < _catalog.entries.Count; i++)
            if (_catalog.entries[i].id == id)
                return i;
        return -1;
    }   

    // --- SERVER LOGIC ---
    public override void FirstAction(bool isPressed = false)
    {
        if (!IsServer)
            return;

        base.FirstAction();                      
    }

    public override void SecondAction(bool isPressed = false)
    {
        if (!IsServer)
            return;

        base.SecondAction();                    
    }

    public override void ThirdAction(bool isPressed = false)
    {
        if (!IsServer)
            return;    

        base.ThirdAction();        
    }

    public override void FourthAction(bool isPressed = false)
    {
        if (!IsServer)
            return;

        base.FourthAction();       
    }

    // --- GLOBAL VISUALS ---
    public override void PlayFirstActionVisuals(bool isPressed = false)
    {            
        base.PlayFirstActionVisuals();           
    }

    public override void PlaySecondActionVisuals(bool isPressed = false)
    {
        base.PlaySecondActionVisuals();              
    }

    public override void PlayThirdActionVisuals(bool isPressed = false)
    {
        base.PlayThirdActionVisuals();           
    }

    public override void PlayFourthActionVisuals(bool isPressed = false)
    {
        base.PlayFourthActionVisuals();        
    }

    // --- LOCAL FEEDBACK ---
    public override void PlayFirstActionLocalFeedback(bool isPressed)
    {
        base.PlayFirstActionLocalFeedback();
        if (isPressed && _propSelectorActive)
        {
            TryPropSelect();
            Debug.Log("Can select prop");            
        }
    }
    public override void PlaySecondActionLocalFeedback(bool isPressed) 
    {
        base.PlaySecondActionLocalFeedback();
        _propSelectorActive = isPressed;

        if (_propSelectorActive)
        {
            HUDManager.Instance?.ShowCrosshair(true);
            Debug.Log("Display targeting HUD");
        }
        if(!_propSelectorActive)
        {
            HUDManager.Instance?.ShowCrosshair(false);
            Debug.Log("HUD Removed");
        }
    }   
    
    public override void PlayThirdActionLocalFeedback(bool isPressed)
    {
        base.PlayThirdActionLocalFeedback();
        if (!isPressed || unlocked.Count == 0) return;

        // Client decides next unlocked entry and asks server to set it.
        int current = _currentCatalogIndex.Value;
        int idxInUnlocked = Mathf.Max(0, unlocked.IndexOf(current));
        int next = unlocked[(idxInUnlocked + 1) % unlocked.Count];
        SetDisguiseServerRpc(next);
    }

    public override void PlayFourthActionLocalFeedback(bool isPressed)
    {
        base.PlayFourthActionLocalFeedback();
        if (!isPressed || unlocked.Count == 0) return;

        int current = _currentCatalogIndex.Value;
        int idxInUnlocked = Mathf.Max(0, unlocked.IndexOf(current));
        int prev = unlocked[(idxInUnlocked - 1 + unlocked.Count) % unlocked.Count];
        SetDisguiseServerRpc(prev);
    }

    public void OnHit()
    {
        if (IsOwner)
        {
            PropDeathServerRpc();
        }
    }

    [ClientRpc]
    private void PropDeathClientRpc(ClientRpcParams rpcParams = default)
    {
        if (!IsServer) Destroy(gameObject);
    }

    [ServerRpc]
    private void PropDeathServerRpc(ServerRpcParams rpcParams = default)
    {       
        PropDeathClientRpc();
        Destroy(gameObject);
    }
        
}
