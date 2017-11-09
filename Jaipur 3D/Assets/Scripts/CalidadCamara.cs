using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CalidadCamara : MonoBehaviour {

    public int nivelCalidad;

    public AjusteCalidadCamara[] ajustes;

    void Awake () {
        nivelCalidad = QualitySettings.GetQualityLevel();

        Camera.main.renderingPath = ajustes[nivelCalidad].renderizado;
        FindObjectOfType<PostProcessingBehaviour>().profile = ajustes[nivelCalidad].perfil;

        if(nivelCalidad == 0) {
            FindObjectOfType<Light>().intensity = 8f;
        }

	}
}

[System.Serializable]
public class AjusteCalidadCamara {
    public RenderingPath renderizado;
    public PostProcessingProfile perfil;
}
