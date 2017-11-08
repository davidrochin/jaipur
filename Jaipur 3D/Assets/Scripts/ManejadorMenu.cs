using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ManejadorMenu : MonoBehaviour {

    public GameObject menuPrincipal;
    public GameObject menuCrear;
    public GameObject menuUnirse;
    public GameObject menuConectarse;
    public GameObject menuPausa;

    void Awake() {
        OcultarTodos();

        if (menuPrincipal != null) { menuPrincipal.SetActive(true); }
    }

    void Update() {

    }

    public void MostrarMenuPrincipal(bool valor) {
        menuPrincipal.SetActive(valor);
    }

    public void MostrarMenuCrear(bool valor) {
        menuCrear.SetActive(valor);
        Text textoIp = GameObject.FindGameObjectWithTag("Texto IP").GetComponent<Text>();
        if (textoIp != null) {
            textoIp.text = ManejadorRed.ObtenerIP();
        }
    }

    public void MostrarMenuUnirse(bool valor) {
        menuUnirse.SetActive(valor);
    }

    public void MostrarMenuPausa(bool valor) {
        menuPausa.SetActive(valor);
    }

    public void MostrarMenuConectarse(bool valor) {
        menuConectarse.SetActive(valor);
    }

    public void OcultarTodos() {
        if (menuPrincipal != null) { menuPrincipal.SetActive(false);  }
        if (menuUnirse != null) { menuUnirse.SetActive(false); }
        if (menuCrear != null) { menuCrear.SetActive(false); }
        if (menuPausa != null) { menuPausa.SetActive(false); }
        if (menuConectarse != null) { menuConectarse.SetActive(false); }
    }

    public void CargarEscena(string nombre) {
        SceneManager.LoadScene(nombre);
    }

    public void SalirDeLaAplicacion() {
        Application.Quit();
    }

}
