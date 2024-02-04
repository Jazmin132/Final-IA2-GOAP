using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanteenFood : MonoBehaviour
{
    public Text[] howMuchCF;

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
    public void HowMuchFoodINeedCF(int apple, int bean, int cocos)
    {
        howMuchCF[0].text = apple.ToString();
        howMuchCF[1].text = bean.ToString();
        howMuchCF[2].text = cocos.ToString();
    }
}
