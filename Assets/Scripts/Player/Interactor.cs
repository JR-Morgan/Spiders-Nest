using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private const float MaxRaycastDistance = 7f;
    private new Camera camera;

    private IInteractable active;

    private Dictionary<KeyCode, Action> actions;

    private void Awake()
    {
        actions = new Dictionary<KeyCode, Action>();
    }

    private void Start()
    {
        camera = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();
    }

    public void AddKeyListener(KeyCode key, Action action)
    {
        actions.Add(key, action);
    }
    public void ClearListeners()
    {
        actions.Clear();
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
        Vector3 CameraCenter = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, camera.nearClipPlane));
        Ray ray = new Ray(CameraCenter, camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, MaxRaycastDistance))
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
                            kvp.Value();
                        }
                    }
                }
                return;
            }
        }

        ResetActive();
    }
}
