using Fusion;

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

public class AccesTitan : NetworkBehaviour
{
    [Networked]
    public NetworkObject TitanObject { get; set; }
    
    [Networked]
    public EnterVanguardTitan TitanScript { get; set; }
    
    [SerializeField] private NetworkPrefabRef _vanguardTitanPrefab;

    PilotMovement moveScript;
    
    float shortestDistance = 0f;
    Vector3 chosenPoint;

    Animator animator;

    private void Start()
    {
        moveScript = GetComponent<PilotMovement>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        StartTitanFall();
        EmbarkWithTitan();
    }
 
    void StartTitanFall()
    {
        if (!HasInputAuthority) return;
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (TitanObject != null)
                Runner.Despawn(TitanObject);
            
            SpawnToDropLocationRPC();
            
            if (TitanScript != null)
                TitanScript.StartFall();
        }
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void SpawnToDropLocationRPC()
    {
        if (Runner.TryGetPlayerObject(Object.InputAuthority, out NetworkObject networkPlayerObject))
        {

            Camera pilotCamera = networkPlayerObject.GetComponentInChildren<Camera>();

            Vector3 direction = pilotCamera.transform.forward;

            if (Physics.Raycast(pilotCamera.transform.position, direction, out RaycastHit hit))
            {
                chosenPoint = hit.point;
            }
            else
            {
                // Fallback just in case.
                chosenPoint = networkPlayerObject.transform.position;
            }

            Vector3 spawnPosition = chosenPoint + new Vector3(0, 150, 0);
            NetworkObject networkPlayerTitanObject =
                Runner.Spawn(_vanguardTitanPrefab, spawnPosition, Quaternion.identity,
                    Runner.LocalPlayer);
            
            if (HasInputAuthority)
            {
                ////networkPlayerTitanObject.gameObject.layer = 6;
                ////LayerUtility.SetLayerRecrusivly(networkPlayerTitanObject.transform);
            }
        }
    }


    void EmbarkWithTitan()
    {
        if (!HasInputAuthority) return;
        
        if (Input.GetKeyDown(KeyCode.F) && TitanScript.inRangeForEmbark)
        {
            StartCoroutine(TitanScript.Embark());

            //moveScript.EmbarkLookDirection(TitanScript.embarkLookTarget.position);

            moveScript.lookTarget = TitanScript.embarkLookTarget.position;
            moveScript.canMove = false;
            moveScript.embarking = true;
            moveScript.embarkPos = TitanScript.embarkPos.position;
            animator.SetTrigger("embark");
        }
    }

    public void ExitTitan()
    {
        moveScript.embarking = false;
        moveScript.canMove = true;
        animator.SetTrigger("exitTitan");
    }

}
