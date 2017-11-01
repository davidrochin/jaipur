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
    public GameObject menuUnirse;

    public GameObject servidorPrefab;
    public GameObject clientePrefab;

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        OcultarTodos();
        menuPrincipal.SetActive(true);
    }

    void Update()
    {

    }

    public void MostrarMenuPrincipal(bool valor)
    {
        OcultarTodos();
        menuPrincipal.SetActive(valor);
    }

    public void MostrarMenuCrear(bool valor)
    {
        OcultarTodos();
        menuCrear.SetActive(valor);
    }

    public void MostrarMenuUnirse(bool valor)
    {
        OcultarTodos();
        menuUnirse.SetActive(valor);
    }

    void OcultarTodos()
    {
        menuPrincipal.SetActive(false);
        menuUnirse.SetActive(false);
        menuCrear.SetActive(false);
    }

    public void IniciarJuego()
    {
        // Cargamos la escena del juego
        // SceneManager.LoadScene("juego");
        SceneManager.LoadScene("prueba");
    }

}
