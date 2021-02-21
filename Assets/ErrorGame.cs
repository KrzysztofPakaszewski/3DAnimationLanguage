using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorGame : MonoBehaviour
{
    public Text ErrorMessage;

    public void SetActive(bool value) {
        gameObject.active = value;
    }
}
