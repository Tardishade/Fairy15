using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CollisionControl : NetworkBehaviour {

    

    public float maxHealth;
    public int bulletDamage;
    public float killingSpeed;
    public float surfaceDamage;
    public int lives;

    public GameObject deathExplosion;
    public GameObject collideExplosion;

    [SyncVar(hook = "ChangeHealthBar")]
    public float health;
    public RectTransform healthBar;

    private Rigidbody rb;
    private NetworkStartPosition[] spawnPoints;

    GameObject deathxmpInst;


    // Use this for initialization
    void Start() {
        if (isLocalPlayer) {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
        rb = transform.GetComponent<Rigidbody>();
        health = maxHealth;
    }

    // Update is called once per frame
    void Update() {
        

    }


    // this is for colliding with pre-built aircraft colliders (not triggers)
    void OnCollisionEnter(Collision collision)
    {
        if (health > 0)
        {

            Debug.Log(collision.gameObject.tag);
            if (collision.gameObject.tag != "Bullet") // must be collision against the environment
            {

                // do a little explosion
                Instantiate(collideExplosion, collision.contacts[0].point, transform.rotation);

                Debug.Log("Collision");

                TakeDamage(surfaceDamage);

                Debug.Log("HEALTH: " + health);
            }

        }
    }


    void OnTriggerEnter(Collider impactObject)
    {
        //Debug.Log("HEALTH");
        if (health > 0)
        {
            Debug.Log(impactObject.tag);
            if (impactObject.tag == "Bullet")
            {
                TakeDamage(bulletDamage);
                Destroy(impactObject);
            }
            
            Debug.Log("HEALTH: " + health);

        }
    }

      IEnumerator Wait() {
        yield return new WaitForSeconds(3);
        Debug.Log("Waited 3 seconds");
    }

    [ClientRpc]
    void RpcRespawn() {
        if (isLocalPlayer)
        {   
            StartCoroutine("Wait");
            //transform.GetComponent<UnityStandardAssets.Vehicles.Aeroplane.AeroplaneController>().enabled = true;
            //transform.GetComponent<UnityStandardAssets.Vehicles.Aeroplane.AeroplaneUserControl2Axis>().enabled = true;
            //rb.useGravity = false;

            //Destroy(deathxmpInst);

            //transform.position = Vector3.zero;
            Vector3 spawnPoint = Vector3.zero;

            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            transform.position = spawnPoint;
        }
    }

  


    void ExplodeAndCrash()
    {
        // instantiate explosion, fire and smoke
        //deathxmpInst = Instantiate(deathExplosion, transform.position, transform.rotation, transform) as GameObject;

        // disable controls and enable gravity on rigidbody
        //transform.GetComponent<UnityStandardAssets.Vehicles.Aeroplane.AeroplaneController>().enabled = false;
        //transform.GetComponent<UnityStandardAssets.Vehicles.Aeroplane.AeroplaneUserControl2Axis>().enabled = false;
        //rb.useGravity = true;
        //rb.mass = 1000;
        //rb.AddForce(Vector3.down * 5000);

        //Physics.gravity = Vector3.down * 100;

        //transform.GetChild(1).gameObject.SetActive(false);

    }

    void ChangeHealthBar(float myHealth) {
        Debug.Log(myHealth);
        healthBar.sizeDelta = new Vector2((myHealth / maxHealth) * 100, healthBar.sizeDelta.y);
    }

    void TakeDamage(float amount){
        if (!isServer) {
            return;
        }
        health -= amount;
        if (health <= 0) {
            ExplodeAndCrash();
            health = maxHealth;
            //score
            RpcRespawn();
        }
        //ChangeHealthBar();
    }

}
