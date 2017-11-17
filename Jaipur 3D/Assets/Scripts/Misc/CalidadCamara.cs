using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CalidadCamara : MonoBehaviour {

    public int nivelCalidad;

    public AjusteCalidadCamara[] ajustes;

    void Awake () {
        //Si es celular, establecer la calidad mas baja
        if(Application.platform == RuntimePlatform.Android) {
            QualitySettings.SetQualityLevel(0);
            AjustarCamaraParaAndroid();
        }

        nivelCalidad = QualitySettings.GetQualityLevel();

        Camera.main.renderingPath = ajustes[nivelCalidad].renderizado;
        FindObjectOfType<PostProcessingBehaviour>().profile = ajustes[nivelCalidad].perfil;

        if(nivelCalidad == 0) {
            FindObjectOfType<Light>().intensity = 13f;
            FindObjectOfType<Light>().spotAngle = 180f;
        }

	}

    void AjustarCamaraParaAndroid() {
        Camera.main.transform.position = new Vector3(0f, 4.31f, -2.99f);
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(68.6f, 0f, -0.002f));
        Camera.main.fieldOfView = 60;
        
    }
}

[System.Serializable]
public class AjusteCalidadCamara {
    public RenderingPath renderizado;
    public PostProcessingProfile perfil;
}
