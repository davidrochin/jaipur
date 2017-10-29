using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.IO;

public class Servidor : MonoBehaviour
{

    public int puerto = 6321;

    private List<ServidorCliente> clientes;
    private List<ServidorCliente> clientesDesconectados;

    private TcpListener servidor;
    private bool servidorIniciado;

    public void Init()
    {
        // Se hace para cambiar de escena ( de menu al juego, se crea el servidor pero no se destruye)
        DontDestroyOnLoad(gameObject);
        clientes = new List<ServidorCliente>();
        clientesDesconectados = new List<ServidorCliente>();

        try
        {
            servidor = new TcpListener(IPAddress.Any, puerto);
            servidor.Start();

            empezarEscuchar();
            servidorIniciado = true;
        }
        catch (Exception e)
        {
            Debug.Log("Error de socket: " + e.Message);
        }
    }

    private void Update()
    {
        if (!servidorIniciado)
            return;
        foreach (ServidorCliente c in clientes)
        {
            // Esta el cliente conectado aun 
            if (!estaConectado(c.tcp))
            {
                c.tcp.Close();
                clientesDesconectados.Add(c);
                continue;
            }
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader lector = new StreamReader(s, true);
                    string datos = lector.ReadLine();

                    if (datos != null)
                    {
                        DatosEntrantes(c, datos);
                    }
                }
            }
        }
        for (int i = 0; i < clientesDesconectados.Count - 1; i++)
        {
            // Le dice a nuestro jugador que alguien se ha desconectado

            clientes.Remove(clientesDesconectados[i]);
            clientesDesconectados.RemoveAt(i);
        }
    }

    private void empezarEscuchar()
    {
        servidor.BeginAcceptTcpClient(aceptarClienteTcp, servidor);
    }

    private void aceptarClienteTcp(IAsyncResult ar)
    {
        TcpListener escucha = (TcpListener)ar.AsyncState;

        string todosLosClientes = "";
        foreach (ServidorCliente i in clientes)
        {
            todosLosClientes += i.clienteNombre + '|';
        }

        ServidorCliente sc = new ServidorCliente(escucha.EndAcceptTcpClient(ar));
        clientes.Add(sc);

        empezarEscuchar();

        Broadcast("SWHO|" + todosLosClientes, clientes[clientes.Count - 1]);
    }

    private bool estaConectado(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    // Enviar desde servidor
    private void Broadcast(string datos, List<ServidorCliente> cl)
    {
        foreach (ServidorCliente sc in cl)
        {
            try
            {
                StreamWriter escritor = new StreamWriter(sc.tcp.GetStream());
                escritor.WriteLine(datos);
                escritor.Flush();
            }
            catch (Exception e)
            {
                Debug.Log("Error de envio : " + e.Message);
            }
        }
    }
    private void Broadcast(string datos, ServidorCliente c)
    {
        List<ServidorCliente> sc = new List<ServidorCliente> { c };
        Broadcast(datos, sc);
    }

    // Leer desde servidor
    private void DatosEntrantes(ServidorCliente c, string datos)
    {
        //Debug.Log(c.clienteNombre + " : " + datos);
        Debug.Log("Servidor: " + datos);
        string[] aData = datos.Split('|');

        switch (aData[0])
        {
            case "CWHO":
                c.clienteNombre = aData[1];
                c.esAnfitrion = (aData[2] == "0") ? false : true;
                Broadcast("SCNN|" + c.clienteNombre, clientes);
                break;
            case "CMOV":
                // Broadcast("SMOV|" + aData[1] + "|" + aData[2] + "|" + aData[3] + "|" + aData[4], clientes);
                Broadcast("SMOV|" + aData[1], clientes);
                break;

        }
    }

}

public class ServidorCliente
{
    public string clienteNombre;
    public TcpClient tcp;
    public bool esAnfitrion;

    public ServidorCliente(TcpClient tcp)
    {
        this.tcp = tcp;
    }
}
