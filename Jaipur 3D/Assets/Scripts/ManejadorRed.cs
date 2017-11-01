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

    public void ConfigurarComoHost() {
        servidor = Instantiate(prefabServidor).GetComponent<Servidor>();
        cliente = Instantiate(prefabCliente).GetComponent<Cliente>();
        cliente.Arrancar(textoIp.text, int.Parse(textoPuerto.text));
    }

    public void ConfigurarComoCliente() {
        cliente = Instantiate(prefabCliente).GetComponent<Cliente>();
        cliente.Arrancar("localhost", int.Parse(textoPuerto.text));
    }
    
}