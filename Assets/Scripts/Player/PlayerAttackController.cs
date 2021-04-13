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
        this.RequireComponentInChildren(out camera);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) //TODO use input system
        {
            Vector3 cameraCenter = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, camera.nearClipPlane));
            Ray ray = new Ray(cameraCenter, camera.transform.forward);
           
            if (Physics.Raycast(ray, out RaycastHit hit, 500f))
            {
                Vector3 position = new Vector3(hit.point.x, 0f, hit.point.z);
                GameObject go = Instantiate(holePrefab, position, Quaternion.identity);
                go.layer = attackLayer.ToLayerNumber();
            }
        }
    }
}
