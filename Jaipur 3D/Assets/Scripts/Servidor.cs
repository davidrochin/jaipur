using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Servidor : MonoBehaviour {

    public static Servidor singleton;

    public int puerto = 8321;

    public bool juegoIniciado = false;

    private void Awake() {

        //Destruirse si ya hay un Servidor
        if (singleton == null) {
            singleton = this;
        } else if (singleton != null && singleton != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        //Arrancar la corrutina que revisa si se desconectó algun jugador
        StartCoroutine(RevisarDesconexiones());

        Debug.Log("Creando servidor...");
        CrearServidor();
    }

    private void Update() {
        if (!juegoIniciado) {
            if (ObtenerNumeroConectados() == 2) {
                Debug.Log("Ya hay 2 jugadores. Iniciando Juego.");
                juegoIniciado = true;
                //SceneManager.LoadScene("juego");
                FindObjectOfType<Cliente>().EnviarAccion(MensajeAccion.TipoAccion.IniciarJuego);
                FindObjectOfType<ManejadorMenu>().CargarEscena("juego");
            }
        }
    }

    public int ObtenerNumeroConectados() {
        int cuenta = 0;
        foreach (NetworkConnection conn in NetworkServer.connections) {
            if (conn != null) {
                cuenta++;
            }
        }
        return cuenta;
    }

    public void CrearServidor() {
        //NetworkServer.RegisterHandler(MensajeString.TIPO, EnviarStringATodos);
        NetworkServer.RegisterHandler(Movimiento.TIPO, EnviarMovimientoAlOtro);
        NetworkServer.RegisterHandler(MensajeAccion.TIPO, EnviarAccionAlOtro);
        NetworkServer.RegisterHandler(Orden.TIPO, EnviarOrdenAlOtro);
        NetworkServer.Listen(puerto);
    }

    /*public void EnviarStringATodos(NetworkMessage mensajeRed) {
        MensajeString msjStr = new MensajeString();
        msjStr = mensajeRed.ReadMessage<MensajeString>();
        NetworkServer.SendToAll(MensajeString.TIPO, msjStr);
    }*/

    public void EnviarMovimientoAlOtro(NetworkMessage mensajeRed) {
        Movimiento msjMov = mensajeRed.ReadMessage<Movimiento>();
        Cliente cliente = FindObjectOfType<Cliente>();
        foreach (NetworkConnection conn in NetworkServer.connections) {
            if (conn != null) {
                if (conn.connectionId != mensajeRed.conn.connectionId) {
                    NetworkServer.SendToClient(conn.connectionId, Movimiento.TIPO, msjMov);
                }
            }
        }
        //NetworkServer.SendToAll(Movimiento.TIPO, msjMov);
    }

    public void EnviarAccionAlOtro(NetworkMessage mensajeRed) {

        MensajeAccion msjAcc = mensajeRed.ReadMessage<MensajeAccion>();
        foreach (NetworkConnection conn in NetworkServer.connections) {
            if (conn != null) {
                if (conn.connectionId != mensajeRed.conn.connectionId) {
                    NetworkServer.SendToClient(conn.connectionId, MensajeAccion.TIPO, msjAcc);
                }
            }
        }
    }

    public void EnviarOrdenAlOtro(NetworkMessage mensajeRed) {
        Orden msjOrden = mensajeRed.ReadMessage<Orden>();
        Cliente cliente = FindObjectOfType<Cliente>();
        foreach (NetworkConnection conn in NetworkServer.connections) {
            if (conn != null) {
                if (conn.connectionId != mensajeRed.conn.connectionId) {
                    NetworkServer.SendToClient(conn.connectionId, Orden.TIPO, msjOrden);
                }
            }
        }
    }

    IEnumerator RevisarDesconexiones() {
        bool revisar = true;
        while (revisar) {
            if (juegoIniciado && ObtenerNumeroConectados() < 2) {
                revisar = false;
                Debug.LogWarning("Se ha desconectado un jugador.");
                FindObjectOfType<CreadorDialogos>().CrearDialogo("Se ha perdido la conexión con el invitado.", CreadorDialogos.AccionBoton.SalirAlMenu);
            }
            yield return new WaitForSeconds(2f);
        }
        
    }
}