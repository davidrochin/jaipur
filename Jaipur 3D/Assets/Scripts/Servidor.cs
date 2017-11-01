using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Servidor : MonoBehaviour
{

    public static Servidor instancia;

    public int PUERTO = 8321;

    public bool juegoIniciado = false;

    private void Awake()
    {

        //Destruirse si ya hay un Servidor
        if (instancia == null)
        {
            instancia = this;
        }
        else if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        Debug.Log("Creando servidor");
        CrearServidor();
    }

    private void Update()
    {
        if (!juegoIniciado)
        {
            if (ObtenerNumeroConectados() == 2)
            {
                Debug.Log("Ya hay 2 jugadores. Iniciando Juego.");
                juegoIniciado = true;
                SceneManager.LoadScene("juego");
            }
        }
    }

    public int ObtenerNumeroConectados()
    {
        int cuenta = 0;
        foreach (NetworkConnection conn in NetworkServer.connections)
        {
            if (conn != null)
            {
                cuenta++;
            }
        }
        return cuenta;
    }

    public void CrearServidor()
    {
        NetworkServer.RegisterHandler(MensajeString.TIPO, EnviarStringATodos);
        NetworkServer.RegisterHandler(Movimiento.TIPO, EnviarMovimientoATodos);
        NetworkServer.Listen(PUERTO);
    }

    public void EnviarStringATodos(NetworkMessage mensajeRed)
    {
        MensajeString msjStr = new MensajeString();
        msjStr = mensajeRed.ReadMessage<MensajeString>();
        NetworkServer.SendToAll(MensajeString.TIPO, msjStr);
    }

    public void EnviarMovimientoATodos(NetworkMessage mensajeRed)
    {
        Movimiento msjMov = mensajeRed.ReadMessage<Movimiento>();
        NetworkServer.SendToAll(Movimiento.TIPO, msjMov);
    }
}