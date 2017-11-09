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

        if (Application.platform != RuntimePlatform.WindowsEditor) return;

        if(cliente != null) {
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
            if(GUILayout.Button("Acabar ronda")) {
                cliente.EnviarAccion(MensajeAccion.TipoAccion.AcabarRonda);
                manejador.AcabarRonda();
            }
            if (GUILayout.Button("Acabar juego")) {
                cliente.EnviarAccion(MensajeAccion.TipoAccion.AcabarJuego);
                manejador.AcabarJuego();
            }
            if (GUILayout.Button("Conseguir todos los camellos")) {
                Carta[] todasCartas = manejador.ObtenerTodasLasCartas();
                foreach (Carta carta in todasCartas) {
                    if (carta.mercancia == Carta.TipoMercancia.Camello) {
                        manejador.DarCartaAJugador(carta);
                    }
                }
            }
        }
        
    }
}
