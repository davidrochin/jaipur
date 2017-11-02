using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Cliente : MonoBehaviour
{

    public NetworkClient cliente;
    public ManejadorJuego manejadorJuego;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Arrancar(string ip, int puerto)
    {
        cliente = new NetworkClient();
        cliente.RegisterHandler(MsgType.Connect, AlConectarse);
        cliente.RegisterHandler(MensajeString.TIPO, ImprimirEnConsola);
        cliente.RegisterHandler(MensajeAccion.TIPO, EjecutarAccion);
        cliente.RegisterHandler(Movimiento.TIPO, EjecutarMovimiento);
        cliente.Connect(ip, puerto);
    }

    public void AlConectarse(NetworkMessage mensajeRed)
    {
        Debug.Log("Conectado al servidor.");
    }

    public void ImprimirEnConsola(NetworkMessage mensajeRed)
    {
        MensajeString msjString = new MensajeString();
        msjString = mensajeRed.ReadMessage<MensajeString>();
        Debug.Log(msjString.mensaje);
    }

    public void EnviarMovimiento(Movimiento mov)
    {
        cliente.Send(Movimiento.TIPO, mov);
    }

    public void EnviarString(string msj)
    {
        MensajeString msjString = new MensajeString();
        msjString.mensaje = msj;
        cliente.Send(MensajeString.TIPO, msjString);
    }

    public void EjecutarAccion(NetworkMessage msjRed)
    {
        MensajeAccion msjAcc = msjRed.ReadMessage<MensajeAccion>();
        Debug.Log(msjAcc.tipoAccion);
        switch (msjAcc.tipoAccion)
        {
            case MensajeAccion.TipoAccion.IniciarJuego:
                SceneManager.LoadScene("juego");
                break;
        }
    }

    public void EjecutarMovimiento(NetworkMessage msjRed)
    {
        Movimiento mov = msjRed.ReadMessage<Movimiento>();
        manejadorJuego.EjecutarMovimientoOponente(mov);
    }
}