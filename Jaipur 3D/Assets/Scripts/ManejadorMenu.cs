using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ManejadorMenu : MonoBehaviour
{

    public static ManejadorMenu Instance { set; get; }

    public GameObject menuPrincipal;
    public GameObject menuCrear;
    public GameObject menuConectar;

    public GameObject servidorPrefab;
    public GameObject clientePrefab;

    public InputField nombreInput;


    // Use this for initialization
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        OcultarTodos();
        menuPrincipal.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BotonAtras()
    {
        OcultarTodos();
        menuPrincipal.SetActive(true);

        Servidor s = FindObjectOfType<Servidor>();
        if (s != null)
            Destroy(s.gameObject);
        Cliente c = FindObjectOfType<Cliente>();
        if (c != null)
            Destroy(c.gameObject);
    }

    public void BotonCrear()
    {
        try
        {
            Servidor s = Instantiate(servidorPrefab).GetComponent<Servidor>();
            s.Init();

            Cliente c = Instantiate(clientePrefab).GetComponent<Cliente>();
            c.clienteNombre = nombreInput.text;
            c.esAnfitrion = true;

            if (c.clienteNombre == "")
                c.clienteNombre = "Anfitrion";
            c.ConectarAlServidor("localhost", 6321);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        OcultarTodos();
        menuCrear.SetActive(true);
    }

    public void BotonUnirse()
    {
        OcultarTodos();
        menuConectar.SetActive(true);
    }

    public void BotonConectarAlServidor()
    {
        string direccionAnfitrion = GameObject.Find("Input_IP").GetComponent<InputField>().text; ;
        if (direccionAnfitrion == "")
            direccionAnfitrion = "localhost"; // LocalHost o 127.0.0.1 es igual

        try
        {
            Cliente c = Instantiate(clientePrefab).GetComponent<Cliente>();
            c.clienteNombre = nombreInput.text;
            if (c.clienteNombre == "")
                c.clienteNombre = "Cliente";
            c.ConectarAlServidor(direccionAnfitrion, 6321);
            menuConectar.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }

    void OcultarTodos()
    {
        menuPrincipal.SetActive(false);
        menuConectar.SetActive(false);
        menuCrear.SetActive(false);
    }

    public void IniciarJuego()
    {
        // Cargamos la escena del juego
        SceneManager.LoadScene("juego");
    }

}
