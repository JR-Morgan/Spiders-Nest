using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
public class Interactor : MonoBehaviour
{
    private const float MAX_RAYCAST_DISTANCE = 7f;
    private new Camera camera;

    private IInteractable active;

    private Dictionary<KeyCode, Action> actions;

    private void Awake()
    {
        actions = new Dictionary<KeyCode, Action>();
        this.RequireComponentInChildren(out camera);

        if (TryGetComponent(out PhotonView photonView))
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected)
            {
                Destroy(this);
                Destroy(camera);
                return;
            }
        }
    }


    public void AddKeyListener(KeyCode key, Action action)
    {
        actions.Add(key, action);
    }
    public void ClearListeners()
    {
        actions?.Clear();
    }

    private void ResetActive(IInteractable interactable = null)
    {
        if (active != null)
        {
            active.OnHoverEnd(this);
        }
        ClearListeners();
        active = interactable;
    }

    public void Update()
    {
        Vector3 cameraCenter = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, camera.nearClipPlane));
        Ray ray = new Ray(cameraCenter, camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, MAX_RAYCAST_DISTANCE))
        {
            GameObject go = hit.collider.gameObject;

            if (go.TryGetComponent(out IInteractable interactable))
            {
                if (active != interactable)
                {
                    ResetActive(interactable);
                    
                    active.OnHoverStart(this);
                }
                else
                {
                    foreach (var kvp in actions)
                    {
                        if (Input.GetKeyDown(kvp.Key))
                        {
                            kvp.Value.Invoke();
                        }
                    }
                }
                return;
            }
        }

        ResetActive();
    }
}
