using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seleccion : MonoBehaviour {

    public List<GameObject> objetosSeleccionados;
    public List<Carta> cartasSeleccionadas;

    public ManejadorJuego manejador;

    void Awake () {
        //Inicializar el List
        objetosSeleccionados = new List<GameObject>();
        cartasSeleccionadas = new List<Carta>();

        manejador = FindObjectOfType<ManejadorJuego>();
    }
	
	void Update () {
		
	}

    public bool AgregarASeleccion(GameObject objeto, bool checarSeleccion) {

        //Revisar si es el turno del jugador
        if (!manejador.turnoJugador) {
            return false;
        }

        //Revisar si ya se seleccionaron las 10 cartas
        if (objetosSeleccionados.Count < 10) {
            Carta carta = objeto.GetComponent<Carta>();

            //Revisar si se seleccionó algun camello del mercado
            if (carta.mercancia == Carta.TipoMercancia.Camello && carta.grupo == manejador.mazoMercado && !seleccionadosTodosCamellosMercado) {
                //LimpiarSeleccion(manejador.mazoMercado);
                //LimpiarSeleccion(manejador.mazoJugadorCamellos);
                LimpiarSeleccion();
                SeleccionarCamellosMercado(true);
                return true;
            } else if (seleccionadosTodosCamellosMercado && carta.mercancia != Carta.TipoMercancia.Camello && carta.grupo == manejador.mazoMercado) {
                SeleccionarCamellosMercado(false);
            }

            //Añadir la carta a la selección
            objetosSeleccionados.Add(objeto);
            carta.SetSeleccionada(true);
            ActualizarReferenciasCartas();

            return true;
        } else {
            //No añadirla y regresar false
            return false;
        }
    }

    public bool RemoverDeSeleccion(GameObject objeto, bool checarSeleccion) {

        Carta carta = objeto.GetComponent<Carta>();

        if (seleccionadosTodosCamellosMercado && carta.mercancia == Carta.TipoMercancia.Camello && carta.grupo == manejador.mazoMercado) {
            SeleccionarCamellosMercado(false);
            return true;
        }

        foreach (GameObject x in objetosSeleccionados) {
            if (objeto == x) {
                objetosSeleccionados.Remove(x);
                carta.SetSeleccionada(false);
                ActualizarReferenciasCartas();

                return true;
            }
        }
        return false;
    }

    public void LimpiarSeleccion() {
        foreach (Carta carta in cartasSeleccionadas) {
            carta.SetSeleccionada(false);
        }
        objetosSeleccionados.Clear();
        ActualizarReferenciasCartas();
        seleccionadosTodosCamellosMercado = false;
    }

    public void LimpiarSeleccion(Grupo mazo) {
        foreach (Carta carta in cartasSeleccionadas) {
            Debug.Log("Limpiando " + carta.name + " de mazo " + mazo.name);
            if (carta.grupo == mazo) {
                carta.SetSeleccionada(false);
            }
        }
        objetosSeleccionados.Clear();
        ActualizarReferenciasCartas();
    }

    public void ActualizarReferenciasCartas() {
        cartasSeleccionadas.Clear();

        foreach (GameObject x in objetosSeleccionados) {
            cartasSeleccionadas.Add(x.GetComponent<Carta>());
        }
    }

    public int[] ObtenerSeleccionadasPorId() {
        List<int> ids = new List<int>();
        foreach (Carta carta in cartasSeleccionadas) {
            ids.Add(carta.id);
        }
        return ids.ToArray();
    }

    bool seleccionadosTodosCamellosMercado = false;

    public void SeleccionarCamellosMercado(bool seleccion) {
        foreach (Transform hijo in manejador.mazoMercado.transform) {

            Carta carta = hijo.GetComponent<Carta>();

            if (carta.mercancia == Carta.TipoMercancia.Camello) {

                if (seleccion == true) {
                    objetosSeleccionados.Add(carta.gameObject);
                    carta.SetSeleccionada(true);
                    seleccionadosTodosCamellosMercado = true;
                } else {
                    objetosSeleccionados.Remove(carta.gameObject);
                    carta.SetSeleccionada(false);
                    seleccionadosTodosCamellosMercado = false;
                }

                ActualizarReferenciasCartas();
            }
        }
    }
}
