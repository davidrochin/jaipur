using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;

public class Cliente : MonoBehaviour
{
    public string clienteNombre;
    public bool esAnfitrion;

    private bool socketListo;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter escritor;
    private StreamReader lector;

    private List<ClienteJuego> jugadores = new List<ClienteJuego>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool ConectarAlServidor(string host, int puerto)
    {
        if (socketListo)
            return false;
        try
        {
            socket = new TcpClient(host, puerto);
            stream = socket.GetStream();
            escritor = new StreamWriter(stream);
            lector = new StreamReader(stream);

            socketListo = true;
        }
        catch (Exception e)
        {
            Debug.Log("Error socket : " + e.Message);
        }

        return socketListo;
    }

    private void Update()
    {
        if (socketListo)
        {
            if (stream.DataAvailable)
            {
                string datos = lector.ReadLine();
                if (datos != null)
                {
                    DatosEntrantes(datos);
                }
            }
        }
    }

    // Enviar mensajes al servidor
    public void enviar(string datos)
    {
        if (!socketListo)
            return;

        escritor.WriteLine(datos);
        escritor.Flush();
    }

    // Leer mensajes del servidor
    private void DatosEntrantes(string datos)
    {
        Debug.Log("Cliente: " + datos);
        string[] aData = datos.Split('|');

        switch (aData[0])
        {
            case "SWHO":
                for (int i = 1; i < aData.Length - 1; i++)
                {
                    UsuarioConectado(aData[i], false);
                }
                enviar("CWHO|" + clienteNombre + '|' + ((esAnfitrion) ? 1 : 0).ToString());
                break;
            case "SCNN":
                UsuarioConectado(aData[1], false);
                break;
            case "SMOV":
                // Movimiento del jugador
                break;

        }
    }
    private void UsuarioConectado(string nombre, bool host)
    {
        ClienteJuego c = new ClienteJuego();
        c.nombre = nombre;

        jugadores.Add(c);

        if (jugadores.Count == 2)
        {
            // Se intancia la escena a cargar desde el manejador del menu
            
        }
    }

    private void CerrarAplicacion()
    {
        cerrarSocket();
    }

    private void Desactivar()
    {
        cerrarSocket();
    }

    private void cerrarSocket()
    {
        if (!socketListo)
            return;

        escritor.Close();
        lector.Close();
        socket.Close();
        socketListo = false;
    }

}

public class ClienteJuego
{
    public string nombre;
    public bool esHost;
}
