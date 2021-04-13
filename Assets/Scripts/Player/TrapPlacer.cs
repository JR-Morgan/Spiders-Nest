using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPlacer : MonoBehaviour
{
    private const float MAX_RAYCAST_DISTANCE = 500f;


    [SerializeField]
    private GameObject trapPrefab;
    [SerializeField]
    private KeyCode placeKey;

    private new Camera camera;

    private void Awake()
    {
        this.RequireComponentInChildren(out camera);
    }


    private GameObject placingObject;


    private bool IsPlacing => placingObject != null;

    private void Update()
    {
        if (IsPlacing)
        {
            //Update position of placingObject
            Vector3 cameraCenter = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, camera.nearClipPlane));
            Ray ray = new Ray(cameraCenter, camera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, MAX_RAYCAST_DISTANCE))
            {
                Vector3 newPosition = hit.point;
                newPosition.y = 0;
                placingObject.transform.position = newPosition;
            }


            if (Input.GetKeyDown(placeKey))
            {
                placingObject = null;
            }
        }
        else
        {
            if (Input.GetKeyDown(placeKey))
            {
                placingObject = Instantiate(trapPrefab);
            }
        }

    }




}
