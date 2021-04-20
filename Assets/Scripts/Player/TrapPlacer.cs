using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPlacer : MonoBehaviour
{
    private const float MAX_RAYCAST_DISTANCE = 500f;


    [SerializeField]
    private ActionType _activeAction;
    [SerializeField]
    private KeyCode placeKey;
    [SerializeField]
    private Color placingColor;

    public ActionType ActiveAction { get => _activeAction;
        set
        {
            _activeAction = value;
            CancelPlace();
        }
    }

    private new Camera camera;

    private void Awake()
    {
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

    private void Start()
    {
        ActionController.Instance.OnActiveChange.AddListener(a => ActiveAction = a);
        ActiveAction = ActionController.Instance.Active;
    }


    private GameObject placingObject;


    private bool IsPlacing => placingObject != null;


    public bool CancelPlace()
    {
        if (!IsPlacing) return false;
        Destroy(placingObject);

        return true;
    }

    private bool CameraHit(out Vector3 newPosition)
    {
        Vector3 cameraCenter = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, camera.nearClipPlane));
        Ray ray = new Ray(cameraCenter, camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, MAX_RAYCAST_DISTANCE))
        {
            newPosition = hit.point;
            newPosition.y = 0;
            return true;
        }

        newPosition = default;
        return false;
    }

    private void Update()
    {
        if (IsPlacing)
        {
            //Update position of placingObject
            if(CameraHit(out Vector3 newPosition))
            {
                placingObject.transform.position = newPosition;
            }

            static float RoundToNearest(float value, float factor) => Mathf.Round(value / factor) * factor;
            
            placingObject.transform.rotation = Quaternion.Euler(0f, RoundToNearest(this.transform.rotation.eulerAngles.y + 30f, 30f) + 60f,0f);

            if (Input.GetKeyDown(placeKey))
            {
                Place();
            }
        }
        else
        {
            if (Input.GetKeyDown(placeKey))
            {

                if (CameraHit(out Vector3 newPosition))
                {
                    GameObject go = Instantiate(ActiveAction.prefab);
                    go.transform.position = newPosition;

                    if (ActiveAction.placeable)
                    {
                        placingObject = go;
                        SetTint(placingColor);
                    }
                }
            }
        }

    }

    private void Place()
    {
        SetTint(Color.white);
        placingObject = null;
    }

    private void SetTint(Color color)
    {   
        foreach (Renderer r in placingObject.GetComponentsInChildren<Renderer>())
        {
            r.material.SetColor("_Color", color);
        }
    }


}
