using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodPatchFood : MonoBehaviour
{
    public Text[] howMuchFPF;

    public void HowMuchFoodINeedFPF(int apple, int bean, int cocos)
    {
        howMuchFPF[0].text = apple.ToString();
        howMuchFPF[1].text = bean.ToString();
        howMuchFPF[2].text = cocos.ToString();
    }
}
