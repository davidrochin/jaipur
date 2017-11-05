using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ManejadorMenu : MonoBehaviour {

    public static ManejadorMenu instancia;

    public GameObject menuPrincipal;
    public GameObject menuCrear;
    public GameObject menuUnirse;
    public GameObject menuPausa;

    void Awake() {
        OcultarTodos();

        if (menuPrincipal != null) { menuPrincipal.SetActive(true); }
    }

    void Update() {

    }

    public void MostrarMenuPrincipal(bool valor) {
        OcultarTodos();
        menuPrincipal.SetActive(valor);
    }

    public void MostrarMenuCrear(bool valor) {
        OcultarTodos();
        menuCrear.SetActive(valor);
    }

    public void MostrarMenuUnirse(bool valor) {
        OcultarTodos();
        menuUnirse.SetActive(valor);
    }

    public void MostrarMenuPausa(bool valor) {
        menuPausa.SetActive(valor);
    }

    void OcultarTodos() {
        if (menuPrincipal != null) { menuPrincipal.SetActive(false);  }
        if (menuUnirse != null) { menuUnirse.SetActive(false); }
        if (menuCrear != null) { menuCrear.SetActive(false); }
        if (menuPausa != null) { menuPausa.SetActive(false); }
    }

    public void CargarEscena(string nombre) {
        SceneManager.LoadScene(nombre);
    }

    public void SalirDeLaAplicacion() {
        Application.Quit();
    }

}
