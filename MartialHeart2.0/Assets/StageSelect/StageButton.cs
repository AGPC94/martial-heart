using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    public Stage stage;

    Button btn;
    Image image;

    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();

        image = GetComponent<Image>();
        image.sprite = stage.portrait;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
