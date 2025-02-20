﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Cliente : MonoBehaviour {

    public NetworkClient cliente;
    public ManejadorJuego manejadorJuego;

    public Coroutine rutinaTimeout;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public void Arrancar(string ip, int puerto) {
        cliente = new NetworkClient();
        cliente.RegisterHandler(MsgType.Connect, AlConectarse);
        cliente.RegisterHandler(MensajeAccion.TIPO, EjecutarAccion);
        cliente.RegisterHandler(Movimiento.TIPO, EjecutarMovimiento);
        cliente.RegisterHandler(MensajeOrden.TIPO, EjecutarOrden);
        cliente.RegisterHandler(MensajeTurno.TIPO, EjecutarTurno);
        Debug.Log("Tratando de conectarse a " + ip + " en el puerto " + puerto);
        rutinaTimeout = StartCoroutine(Timeout());
        cliente.Connect(ip, puerto);
    }

    public void AlConectarse(NetworkMessage mensajeRed) {
        StopCoroutine(rutinaTimeout);
        Debug.Log("Conectado al servidor.");
        StartCoroutine(RevisarConexionAServidor());
    }

    public void ImprimirEnConsola(NetworkMessage mensajeRed) {
        MensajeString msjString = new MensajeString();
        msjString = mensajeRed.ReadMessage<MensajeString>();
        Debug.Log(msjString.mensaje);
    }

    public void EnviarMovimiento(Movimiento mov) {
        cliente.Send(Movimiento.TIPO, mov);
    }

    public void EnviarString(string msj) {
        MensajeString msjString = new MensajeString();
        msjString.mensaje = msj;
        cliente.Send(MensajeString.TIPO, msjString);
    }

    public void EnviarAccion(MensajeAccion.TipoAccion accion) {
        MensajeAccion msjAcc = new MensajeAccion();
        msjAcc.tipoAccion = accion;
        cliente.Send(MensajeAccion.TIPO, msjAcc);
    }

    public void EnviarOrden(MensajeOrden orden) {
        cliente.Send(MensajeOrden.TIPO, orden);
    }

    public void EnviarTurno(MensajeTurno turno) {
        cliente.Send(MensajeTurno.TIPO, turno);
    }

    public void EjecutarAccion(NetworkMessage msjRed) {
        MensajeAccion msjAcc = msjRed.ReadMessage<MensajeAccion>();
        Debug.Log(msjAcc.tipoAccion);
        switch (msjAcc.tipoAccion) {
            case MensajeAccion.TipoAccion.IniciarJuego:
                SceneManager.LoadScene("juego");
                break;
            case MensajeAccion.TipoAccion.AcabarRonda:
                manejadorJuego.AcabarRonda();
                break;
            case MensajeAccion.TipoAccion.AcabarJuego:
                manejadorJuego.AcabarJuego();
                break;
        }
    }

    public void EjecutarMovimiento(NetworkMessage msjRed) {
        Movimiento mov = msjRed.ReadMessage<Movimiento>();
        manejadorJuego.EjecutarMovimientoOponente(mov);
    }

    public void EjecutarOrden(NetworkMessage msjRed) {
        MensajeOrden orden = msjRed.ReadMessage<MensajeOrden>();
        manejadorJuego.EjecutarOrden(orden);
    }

    public void EjecutarTurno(NetworkMessage msjRed) {
        MensajeTurno turno = msjRed.ReadMessage<MensajeTurno>();
        Debug.Log("Se recibió el turno " + turno.jugador.ToString());
        manejadorJuego.EjecutarTurno(turno);
    }

    IEnumerator RevisarConexionAServidor() {
        bool revisar = true;
        while (revisar) {
            if(cliente.isConnected == false) {
                Debug.LogWarning("Se ha perdido la conexión con el servidor.");
                FindObjectOfType<CreadorDialogos>().CrearDialogo("Se ha perdido la conexión con el anfitrión de la partida.", CreadorDialogos.AccionBoton.SalirAlMenu);
                revisar = false;
            }
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator Timeout() {
        yield return new WaitForSeconds(10f);
        FindObjectOfType<ManejadorRed>().CerrarTodo();
        FindObjectOfType<ManejadorMenu>().MostrarMenuConectarse(false);
        FindObjectOfType<CreadorDialogos>().CrearDialogo("Tiempo de espera agotado.", CreadorDialogos.AccionBoton.Nada);
    }

}