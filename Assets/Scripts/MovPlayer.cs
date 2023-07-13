using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovPlayer : MonoBehaviour
{

	float bForce = 10f;
  float rForce = 100f;
  public CharacterController cc;
  public bool stop = false;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
      if (!stop) { //Comprobación del tiempo restante
        Vector3 move = transform.forward;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        //Movimiento del personaje
        transform.Rotate(Vector3.up * rForce * x * Time.deltaTime, Space.World); //Velocidad de giro horizontal
        transform.Rotate(Vector3.right * rForce * y * Time.deltaTime); //Velocidad de giro vertical
        cc.Move(move * bForce * Time.deltaTime); //Velocidad lineal

        //Rotación del personaje
        /*if (Input.GetKey("q")) {
          pb.Rotate(Vector3.forward * rForce * 1 * Time.deltaTime);
        }
        if (Input.GetKey("e")) {
          pb.Rotate(Vector3.forward * rForce * -1 * Time.deltaTime);
        }*/
      }
    }
  
}
