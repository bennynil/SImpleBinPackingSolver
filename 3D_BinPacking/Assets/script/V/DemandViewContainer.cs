using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class DemandViewContainer : MonoBehaviour
{
    public TextMeshProUGUI name;
    public TextMeshProUGUI description;
    public TextMeshProUGUI descript;

    public void setDescription(string _name, string xyz, string q)
    {
        name.text = _name;
        description.text = "xyz size: " + xyz;
        if(descript != null)
        {
            descript.text = "quantity: " + q;
        }
    }
}
