using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    //[SerializeField]
    //private float damage;
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 CameraCenter = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, camera.nearClipPlane));
            Ray ray = new Ray(CameraCenter, camera.transform.forward);
           
            if (Physics.Raycast(ray, out RaycastHit hit, 500))
            {
                Debug.DrawLine(ray.origin, hit.point);

                Vector3 position = new Vector3(hit.point.x, 0f, hit.point.z);
                Instantiate(holePrefab, position, Quaternion.identity);

            }
        }
    }
}
