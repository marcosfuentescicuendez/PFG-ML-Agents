using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCarrier : MonoBehaviour
{

    public GameObject child;
    public Rigidbody rb;

    public bool stop = false;   
    float force = 2000f;
    public string fatherAsleep;
    float SleepTime = 2.0f;
    public bool shoot;

    // Start is called before the first frame update
    void Start()
    {
        fatherAsleep = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (!stop) { //Comprobación del tiempo restante
            if (fatherAsleep != "") { // Dormido hasta que pase un tiempo
                SleepTime -= Time.deltaTime;

                if (SleepTime <= 0.0f)
                {
                    fatherAsleep = "";
                }
            }

            /*BLOQUE (para que la bola salga recta)*/

            if (child.transform.parent != null && shoot) { //Lanzamiento de la bola
                transform.localRotation = child.transform.parent.rotation;
                rb.isKinematic = false;
                rb.AddForce(transform.forward * 50 * force * Time.deltaTime);
                fatherAsleep = child.transform.parent.name;
                SleepTime = 2.0f;
                child.transform.SetParent(null);
            }
        }
    }

    void OnTriggerEnter(Collider objX) {
        if (!stop) { //Comprobación del tiempo restante
            if (objX.gameObject.name.Contains("Character") && objX.gameObject.name != fatherAsleep) { //Choca y adquiere la bola   
                //Si tiene padre, lo duerme un tiempo X y se le asigna el nuevo padre
                if (child.transform.parent != null && objX.gameObject.name != child.transform.parent.name) {
                    rb.isKinematic = true;
                    fatherAsleep = child.transform.parent.name;
                    //Debug.Log(fatherAsleep);
                    transform.SetPositionAndRotation(objX.transform.localPosition+1*objX.transform.forward, objX.transform.localRotation);
                    child.transform.SetParent(null);
                    child.transform.SetParent(objX.gameObject.transform);
                    SleepTime = 2.0f;
                //Si no tiene padre se le asigna uno
                } else if (child.transform.parent == null) {
                    rb.isKinematic = true;
                    transform.SetPositionAndRotation(objX.transform.localPosition+1*objX.transform.forward, objX.transform.localRotation);
                    child.transform.SetParent(objX.gameObject.transform);
                }
            }
        }
    }

}