using UnityEngine;

namespace Bin.Player
{
    public class CameraController : MonoBehaviour
    {
        private const float Speed = 20f;

        private void Update()
        {
            var speed = Speed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed *= 3;
            }

            if (Input.GetKey(KeyCode.W))
            {
                transform.position += Vector3.forward * (speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= Vector3.forward * (speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += Vector3.left * (speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.right * (speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Space))
            {
                transform.position += Vector3.up * (speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                transform.position += Vector3.down * (speed * Time.deltaTime);
            }
        }
    }
}