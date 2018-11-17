using UnityEngine;

public class PlayerMovement : MonoBehaviour
{     
    [SerializeField] Transform Crosshair;
    [SerializeField] Rigidbody rb;

    //[SerializeField] float xAxisSpeedModifier = 100f;
    //[SerializeField] float horizontalRotationModifier = 10f;
    [SerializeField] float zAxisSpeedModifier = 10f;

    private void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        float z = Input.GetAxis("Vertical") * zAxisSpeedModifier;
        
        rb.velocity = (transform.forward * z) * Time.fixedDeltaTime;
        Vector3 lookRotationVector = Crosshair.position - transform.position;
        if(lookRotationVector != Vector3.zero)
        {
            Quaternion quat = Quaternion.LookRotation(lookRotationVector);
            quat = Quaternion.Slerp(transform.rotation, quat, 10 * Time.deltaTime);
            quat = Quaternion.Euler(0, quat.eulerAngles.y, 0);
            transform.rotation = quat;
        }
    }
}
