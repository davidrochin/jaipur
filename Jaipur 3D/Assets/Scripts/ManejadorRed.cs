using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ManejadorRed : MonoBehaviour {

    public GameObject prefabServidor;
    public GameObject prefabCliente;

    public Text textoIp;
    public Text textoPuerto;

    Servidor servidor;
    Cliente cliente;

    private void Awake() {
        if(FindObjectOfType<ManejadorRed>() != null && FindObjectOfType<ManejadorRed>() != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void ConfigurarComoHost() {
        servidor = Instantiate(prefabServidor).GetComponent<Servidor>();
        cliente = Instantiate(prefabCliente).GetComponent<Cliente>();

        if(textoIp != null && textoPuerto != null) {
            cliente.Arrancar(textoIp.text, int.Parse(textoPuerto.text));
        } else {
            Debug.LogWarning("No se encontró el InputField de la IP y Puerto asi que se conectó a: localhost 8321.");
            cliente.Arrancar("localhost", 8321);
        }
        
    }

    public void ConfigurarComoCliente() {
        cliente = Instantiate(prefabCliente).GetComponent<Cliente>();
        if (textoIp != null && textoPuerto != null) {
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