using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MovCam : MonoBehaviour
{
    public GameObject targetObject;


    public List<Transform> cityLimits;
   
    Vector3 targetTransform;

    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        targetObject = player;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       
        
        if ((player.transform.position.y > cityLimits[2].position.y || player.transform.position.y < cityLimits[3].position.y) && (player.transform.position.x > cityLimits[0].position.x || player.transform.position.x < cityLimits[1].position.x))
        {
            //Camera da cidade X e Y
            targetTransform = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10);
        }
        else if (player.transform.position.x > cityLimits[0].position.x || player.transform.position.x < cityLimits[1].position.x)
        {
            //Camera da cidade X
            targetTransform = new Vector3(gameObject.transform.position.x, targetObject.transform.position.y, -10);

        }
        else if (player.transform.position.y > cityLimits[2].position.y || player.transform.position.y < cityLimits[3].position.y)
        {
            //Camera da cidade Y
            targetTransform = new Vector3(targetObject.transform.position.x, gameObject.transform.position.y, -10);

        }
        //Camera livre
        else { targetTransform = player.transform.position; gameObject.GetComponent<Camera>().orthographicSize = 6.5f; }


        transform.position = Vector3.Lerp(this.transform.position, new Vector3(targetTransform.x, targetTransform.y, -10), 5 * Time.deltaTime);
    }
}
