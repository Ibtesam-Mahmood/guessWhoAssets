using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine {
    public class ToggleCameraAngle : MonoBehaviour
    {
        private bool toggled = false;
        public GameObject go;
        // Start is called before the first frame update
        void Start()
        {
        
        }
    
        // Update is called once per frame
        void Update()
        {
            // print(Input.inputString);
            if (Input.GetButtonDown("Player1_Dash"))
            {
                go.SetActive(toggled);
                toggled = !toggled;
            }
        }
    }
}