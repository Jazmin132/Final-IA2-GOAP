using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanteenFood : MonoBehaviour
{
    public Text[] HowMuch;

    public void HowMuchFoodIneed(int apple, int beam, int cocos)
    {
        HowMuch[0].text = apple.ToString();
        HowMuch[1].text = beam.ToString();
        HowMuch[2].text = cocos.ToString();
    }
}
