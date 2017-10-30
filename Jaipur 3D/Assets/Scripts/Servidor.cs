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

    private Movimiento movObjeto;

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
                    //StreamReader lector = new StreamReader(s, true);
                    //string datos = lector.ReadLine();
                    
                    byte[] movimiento = ReadToEnd(s);
                    //string datosMov = lectorBin.ReadString();

                    if (movimiento != null)
                    {
                        //DatosEntrantes(c, datos);
                        MovimientosEntrantes(c, movimiento);
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

        //Broadcast("SWHO|" + todosLosClientes, clientes[clientes.Count - 1]);
        int[] prob = { -5 };
        Movimiento m = new Movimiento(Movimiento.TipoMovimiento.Ninguno, prob);
        BroadcastMov(m.Serializar(), clientes);
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

    private void BroadcastMov(byte[] datos, List<ServidorCliente> cl)
    {
        foreach (ServidorCliente sc in cl)
        {
            try
            {
                BinaryWriter escritorBin = new BinaryWriter(sc.tcp.GetStream());
                escritorBin.Write(datos);
                escritorBin.Flush();

                //StreamWriter escritor = new StreamWriter(sc.tcp.GetStream());
                ///escritor.WriteLine(datos);
                //escritor.Flush();
            }
            catch (Exception e)
            {
                Debug.Log("Error de envio : " + e.Message);
            }
        }
    }

    private void BroadcastMov(byte[] datos, ServidorCliente c)
    {
        List<ServidorCliente> sc = new List<ServidorCliente> { c };
        BroadcastMov(datos, sc);
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

    private void MovimientosEntrantes(ServidorCliente c, byte[] datos)
    {
        BroadcastMov(datos, clientes);
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

        
    public static byte[] ReadToEnd(System.IO.Stream stream)
    {
        long originalPosition = 0;

        if(stream.CanSeek)
        {
             originalPosition = stream.Position;
             stream.Position = 0;
        }

        try
        {
            byte[] readBuffer = new byte[4096];

            int totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
            {
                totalBytesRead += bytesRead;

                if (totalBytesRead == readBuffer.Length)
                {
                    int nextByte = stream.ReadByte();
                    if (nextByte != -1)
                    {
                        byte[] temp = new byte[readBuffer.Length * 2];
                        Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                        Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                        readBuffer = temp;
                        totalBytesRead++;
                    }
                }
            }

            byte[] buffer = readBuffer;
            if (readBuffer.Length != totalBytesRead)
            {
                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
            }
            return buffer;
        }
        finally
        {
            if(stream.CanSeek)
            {
                 stream.Position = originalPosition; 
            }
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
