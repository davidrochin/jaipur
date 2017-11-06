using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ManejadorRed : MonoBehaviour {

    public static ManejadorRed singleton;

    public GameObject prefabServidor;
    public GameObject prefabCliente;

    public Text textoIp;
    public Text textoPuerto;

    Servidor servidor;
    Cliente cliente;

    private void Awake() {
        //Destruirse si ya hay un ManejadorRed
        if (singleton == null) {
            singleton = this;
        } else if (singleton != null && singleton != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
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
}