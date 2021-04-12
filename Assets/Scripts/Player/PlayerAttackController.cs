using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField]
    private LayerMask attackLayer;
    [SerializeField]
    private GameObject holePrefab;

    private new Camera camera;
    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        if (camera == null) Debug.LogWarning($"Could not find {typeof(Camera)} component in children", this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) //TODO use input system
        {
            Vector3 CameraCenter = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, camera.nearClipPlane));
            Ray ray = new Ray(CameraCenter, camera.transform.forward);
           
            if (Physics.Raycast(ray, out RaycastHit hit, 500f))
            {
                Debug.DrawLine(ray.origin, hit.point);

                Vector3 position = new Vector3(hit.point.x, 0f, hit.point.z);
                GameObject go = Instantiate(holePrefab, position, Quaternion.identity);
                go.layer = attackLayer.ToLayerNumber();
            }
        }
    }
}
