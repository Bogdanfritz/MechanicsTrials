using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {
    [SerializeField] Transform player;
    [SerializeField] Vector3 positionOffset;
	// Use this for initialization
	void Start () {
        transform.position = player.position + positionOffset;
        transform.rotation = player.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = player.position + positionOffset;
        transform.rotation = player.rotation;
    }
}
