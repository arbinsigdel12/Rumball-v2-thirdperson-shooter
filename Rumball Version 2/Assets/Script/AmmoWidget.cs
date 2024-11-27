using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoWidget : MonoBehaviour
{
    public TMPro.TMP_Text ammotext;
    public void Refresh(int ammoCount){
        ammotext.text=ammoCount.ToString();
    }
}
