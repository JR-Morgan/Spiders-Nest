using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This <see cref="MonoBehaviour"/> controls the placing of traps / player attacks.<br/>
/// </summary>
[RequireComponent(typeof(PlayerInventory))]
public class TrapPlacer : MonoBehaviour
{
    private const float MAX_RAYCAST_DISTANCE = 500f;

    [SerializeField]
    private ActionType _activeAction;
    [SerializeField]
    [Tooltip("The key to start placing the " + nameof(ActiveAction))]
    private KeyCode placeKey;
    [Tooltip("The " + nameof(Color) + " to be used while placing for " + nameof(ActionType) + "s where " + nameof(ActionType.isPlaceable) + " is true")]
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
    private PlayerInventory inventory;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        this.RequireComponentInChildren(out camera);

        if (TryGetComponent(out PhotonView photonView))
        {
            if (PhotonNetwork.IsConnected && !photonView.IsMine)
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
        inventory.OnValueChange.AddListener(MoneyChangeHandler);
        ActiveAction = ActionController.Instance.Active;
    }


    private GameObject placingObject;


    private bool IsPlacing => placingObject != null;

    /// <summary>
    /// Cancels the <see cref="ActiveAction"/>
    /// </summary>
    /// <returns><c>false</c> if player was not placing a <see cref="GameObject"/>; otherwise, <c>true</c></returns>
    /// <remarks>See <see cref="IsPlacing"/></remarks>
    public bool CancelPlace()
    {
        if (!IsPlacing) return false;
        Destroy(placingObject);

        return true;
    }

    private void MoneyChangeHandler(float money = default)
    {
        if (IsPlacing)
        {
            if(!inventory.CanAfford(ActiveAction))
            {
                CancelPlace();
            }
        }
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
            if (CameraHit(out Vector3 newPosition))
            {
                placingObject.transform.position = newPosition;
            }

            static float RoundToNearest(float value, float factor) => Mathf.Round(value / factor) * factor;

            placingObject.transform.rotation = Quaternion.Euler(0f, RoundToNearest(this.transform.rotation.eulerAngles.y + NewLevelGenerator.THETA / 2, NewLevelGenerator.THETA / 2) + NewLevelGenerator.THETA, 0f);
            if (Input.GetKeyDown(placeKey))
            {
                if (inventory.TrySubtract(inventory.CostOfAction(ActiveAction)))
                    Place();
            }
        }
        else
        {
            if (Input.GetKeyDown(placeKey))
            {
                if (inventory.CanAfford(ActiveAction)) //Safe to do unchecked inventory transactions inside body
                {
                    if (CameraHit(out Vector3 newPosition))
                    {
                        GameObject go;
                        if (ActiveAction.isPlaceable)
                        {
                            //Local only instantiation while placing
                            go = Instantiate(ActiveAction.prefab, newPosition, Quaternion.identity);
                            placingObject = go;
                            SetColour(placingColor);
                        }
                        else
                        {
                            inventory.SubtractUnchecked(inventory.CostOfAction(ActiveAction)); //Safe to do

                            SetupLocalTrap(InstantiateActive(newPosition, Quaternion.identity));
                        }
                    }
                }
            }
        }
    }


    private GameObject InstantiateActive(Vector3 position, Quaternion rotation)
    {
        if (PhotonNetwork.IsConnected)
        {
            return PhotonNetwork.Instantiate($"Prefabs/Traps/{ActiveAction.prefab.name}", position, rotation);
        }
        else
        {
            return Instantiate(ActiveAction.prefab, position, rotation);
        }
    }

    private void Place()
    {
        Vector3 pos = placingObject.transform.position;
        Quaternion rot = placingObject.transform.rotation;

        Destroy(placingObject);
        placingObject = null;

        GameObject placedGameObject = InstantiateActive(pos, rot);
        SetupLocalTrap(placedGameObject);

    }

    private void SetupLocalTrap(GameObject go)
    {
        if (go.TryGetComponentInChildren(out AOEDamage aoeDamage, true))
        {
            aoeDamage.Damage = ActiveAction.damage;
        }
    }

    private void SetColour(Color color)
    {   
        foreach (Renderer r in placingObject.GetComponentsInChildren<Renderer>())
        {
            r.material.SetColor("_Color", color); //TODO set colour properly
        }
    }


}
