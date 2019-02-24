using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//TODO LATER:: check if we need to derive from network behaviour
public class GunCharacter : NetworkBehaviour {

    //TODO LATER:: change from public, create getter & setter
    public float fireRate;
    public float damage;
    public float magCapacity;
}
