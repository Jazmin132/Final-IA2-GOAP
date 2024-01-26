using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanteenFood : MonoBehaviour
{
    public Text[] HowMuch;
    Transform MainCamera;
    Transform Unit;
    Transform WorldSpaceCanvas;

    public Vector3 Offset;
    /*
    void Start()
    {
        MainCamera = MainCamera.transform;
        Unit = transform.parent;
        WorldSpaceCanvas = GameObject.FindObjectOfType<Canvas>().transform;

        transform.SetParent(WorldSpaceCanvas);

        transform.rotation = Quaternion.LookRotation(transform.position - MainCamera.transform.position);
        transform.position = Unit.position + Offset;
    }
    */
    public void HowMuchFoodIneed(int apple, int beam, int cocos)
    {
        HowMuch[0].text = apple.ToString();
        HowMuch[1].text = beam.ToString();
        HowMuch[2].text = cocos.ToString();
    }
}
