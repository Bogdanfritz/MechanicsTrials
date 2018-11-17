using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using tetherType = Constants.TetherMode;

public class CrosshairBehaviour : MonoBehaviour {
    [SerializeField] float maxRadiusAroundPlayer = 10f;
    [SerializeField] GameObject player;
    bool shouldCreateTether = false;
    bool bumperInUse = false;
    tetherType tetherMode = tetherType.Invalid;

	// Use this for initialization
	void Start () {
    }
    // Update is called once per frame
    void Update() {

        UpdateMovement();
        UpdateTetherInput();
        TryUpdateTether();
    }

    private void ChangeTetherMode(tetherType mode)
    {
        if(mode == tetherType.Invalid)
        {
            shouldCreateTether = false;
        }
        tetherMode = mode;
    }

    private void UpdateTetherInput()
    {
        float rightBumperValue = Input.GetAxisRaw("RightBumper");
        float leftBumperValue = Input.GetAxisRaw("LeftBumper");

        //bumper events
        if (rightBumperValue == 1f && tetherMode == tetherType.Invalid && !bumperInUse)
        {
            ChangeTetherMode(tetherType.PullPlayerToObject);
            shouldCreateTether = !shouldCreateTether;
            bumperInUse = true;
        }
        
        if (leftBumperValue == 1f && tetherMode == tetherType.Invalid && !bumperInUse)
        {
            ChangeTetherMode(tetherType.PullObjectToPlayer);
            shouldCreateTether = !shouldCreateTether;
            bumperInUse = true;
        }

        if (rightBumperValue == 0f && leftBumperValue == 0f && bumperInUse)
        {
            bumperInUse = false;
            tetherMode = tetherType.Invalid;
        }
    }

    private void TryUpdateTether()
    {
        if(shouldCreateTether && bumperInUse)
        {
            player.GetComponent<Tether>().TryInitialize(player.transform.GetChild(0).gameObject, transform.position, tetherMode);
        }
        if (!shouldCreateTether && bumperInUse)
        {
            player.GetComponent<Tether>().TryRetractTether(Time.time);
        }
        
        /*
        if (!tetherCreated && shouldCreateTether && tetherMode != tetherType.Invalid)
        {

            sphereCollider.enabled = true;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, transform.localScale.x);
            foreach (Collider target in hitColliders)
            {
                if (target.tag == "Hookable")
                {
                    GameObject tetheredObject = target.gameObject;
                    tetherCreated = player.GetComponent<Tether>().Initialize(player, tetheredObject, tetherBase, tetherMode);
                }
            }
            
            sphereCollider.enabled = false;
            if (!tetherCreated)
            {
                ChangeTetherMode(tetherType.Invalid);
            }
        }
        else if (tetherCreated && !shouldCreateTether)
        {
            player.GetComponent<Tether>().DestroyTether();
            tetherCreated = false;
            ChangeTetherMode(tetherType.Invalid);
        }*/
    }

    private void UpdateMovement()
    {
        //movement
        float xMovement = Input.GetAxisRaw("RightH");
        float zMovement = Input.GetAxisRaw("RightV");
        Vector3 newPosition = new Vector3(xMovement * maxRadiusAroundPlayer, 0f, zMovement * maxRadiusAroundPlayer);
        Vector3 centerPosition = player.transform.position + player.transform.forward * 2;
        transform.position = centerPosition + newPosition;
    }

    //Fun stuff - Everybody dance !
    //Instantiate(player, transform.position, Quaternion.identity);

}
