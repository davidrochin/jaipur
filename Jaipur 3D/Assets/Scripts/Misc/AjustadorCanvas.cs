using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AjustadorCanvas : MonoBehaviour {

    CanvasScaler escaladorCanvas;

    private void Awake() {
        escaladorCanvas = GetComponent<CanvasScaler>();

        //Para Android
        if(Application.platform == RuntimePlatform.Android) {
            escaladorCanvas.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            escaladorCanvas.referenceResolution = new Vector2(1366f, 768f);
        }

    }
}
