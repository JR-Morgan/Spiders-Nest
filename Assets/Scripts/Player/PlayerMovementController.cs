using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    private CharacterController controller;

    [SerializeField]
    private float speed = 12f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if(TryGetComponent(out PhotonView photonView))
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected)
            {
                Destroy(this);
                Destroy(controller);
                return;
            }
        }
    }


    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

    }
}
