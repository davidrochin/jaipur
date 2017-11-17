using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonesDebug : MonoBehaviour {

    ManejadorJuego manejador;
    Servidor servidor;
    Cliente cliente;

    public bool activado = false;

    private void Awake() {
        manejador = FindObjectOfType<ManejadorJuego>();
        servidor = FindObjectOfType<Servidor>();
        cliente = FindObjectOfType<Cliente>();
    }

    private void OnGUI() {

        if (!activado) return;

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
            if (GUILayout.Button("Vender todas las cartas")) {
                List<int> ids = new List<int>();
                List<Carta> cartas = new List<Carta>(FindObjectsOfType<Carta>());
                Movimiento mov = new Movimiento();
                mov.tipoMovimiento = Movimiento.TipoMovimiento.Vender;
                manejador.DarFichasPorCartas(cartas.ToArray(), manejador.fichasJugador);
                foreach (Carta carta in cartas) {
                    ids.Add(carta.id);
                    carta.EnviarAGrupo(manejador.mazoDescartar);
                }
                mov.ids = ids.ToArray();
                cliente.EnviarMovimiento(mov);
            }
        }
        
    }

    public void AlternarActivado() {
        activado = !activado;
    }
}
