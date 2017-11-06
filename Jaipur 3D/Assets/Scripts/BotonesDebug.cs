using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonesDebug : MonoBehaviour {

    ManejadorJuego manejador;
    Servidor servidor;
    Cliente cliente;

    private void Awake() {
        manejador = FindObjectOfType<ManejadorJuego>();
        servidor = FindObjectOfType<Servidor>();
        cliente = FindObjectOfType<Cliente>();
    }

    private void OnGUI() {
        if(cliente != null && Application.platform == RuntimePlatform.WindowsEditor) {
            if (GUILayout.Button("Destruir casi todas las fichas")) {
                foreach (Grupo grupo in FindObjectsOfType<Grupo>()) {
                    if(grupo.tipoGrupo == Grupo.TipoGrupo.Fichas) {
                        for (int x = 1; x < grupo.transform.childCount; x++) {
                            Destroy(grupo.transform.GetChild(x).gameObject);
                        }
                    }
                }
            }
            if (GUILayout.Button("Forzar turno")) {
                manejador.EmpezarTurno();
            }
        }
        
    }
}
