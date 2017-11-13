using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;

public class ManejadorRed : MonoBehaviour {

    public GameObject prefabServidor;
    public GameObject prefabCliente;

    public Text textoIp;
    public Text textoPuerto;

    Servidor servidor;
    Cliente cliente;

    private void Awake() {
        ObtenerIP();
    }

    public void ConfigurarComoHost() {

        CerrarTodo();

        servidor = Instantiate(prefabServidor).GetComponent<Servidor>();
        cliente = Instantiate(prefabCliente).GetComponent<Cliente>();

        cliente.Arrancar("localhost", 8321);

    }

    public void ConfigurarComoCliente() {

        CerrarTodo();

        cliente = Instantiate(prefabCliente).GetComponent<Cliente>();
        if (textoIp != null && textoPuerto != null) {

            //Revisar que el InputField de la IP no esté vacio
            if (textoIp.text.Trim().Equals("")) { textoIp.GetComponentInParent<InputField>().text = "localhost"; }

            //Guardar la IP como ultima ingresada
            PlayerPrefs.SetString("ultima_ip", textoIp.text);

            //Arrancar el cliente
            cliente.Arrancar(textoIp.text, int.Parse(textoPuerto.text));
        } else {
            Debug.LogWarning("No se encontró el InputField de la IP y Puerto asi que se conectó a: localhost 8321.");
            cliente.Arrancar("localhost", 8321);
        }
    }

    public void ConfigurarComoPrueba() {
        ConfigurarComoHost();
        cliente = Instantiate(prefabCliente).GetComponent<Cliente>();
        if (textoIp != null && textoPuerto != null) {
            //Revisar que el InputField de la IP no esté vacio
            if (textoIp.text.Trim().Equals("")) { textoIp.GetComponentInParent<InputField>().text = "localhost"; }
            cliente.Arrancar(textoIp.text, int.Parse(textoPuerto.text));
        } else {
            Debug.LogWarning("No se encontró el InputField de la IP y Puerto asi que se conectó a: localhost 8321.");
            cliente.Arrancar("localhost", 8321);
        }
    }
    
    public void CerrarTodo() {
        Cliente[] clientes = FindObjectsOfType<Cliente>();
        Servidor[] servidores = FindObjectsOfType<Servidor>();
        if(clientes != null) {
            foreach (Cliente c in clientes) { Destroy(c.gameObject); }
        }
        if (servidores != null) {
            foreach (Servidor s in servidores) { Destroy(s.gameObject); }
        }

        NetworkServer.Shutdown();
        NetworkClient.ShutdownAll();
    }

    public void DesactivarCorrutinasDesconexion() {
        foreach (Servidor s in FindObjectsOfType<Servidor>()) {
            s.StopAllCoroutines();
        }
        foreach (Cliente c in FindObjectsOfType<Cliente>()) {
            c.StopAllCoroutines();
        }
    }

    public void CargarUltimaIP() {
        if(textoIp != null) {
            textoIp.GetComponentInParent<InputField>().text = PlayerPrefs.GetString("ultima_ip", "");
        }
    }

    public static string ObtenerIP() {
        IPHostEntry host;
        string ipLocal = "";
        Debug.Log(Dns.GetHostName());
        host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (IPAddress direccion in host.AddressList) {
            if(direccion.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                ipLocal = direccion.ToString();
                break;
            }
        }
        return ipLocal;
    }
}