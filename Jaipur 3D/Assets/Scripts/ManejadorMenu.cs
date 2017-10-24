using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ManejadorMenu : MonoBehaviour {

    public GameObject menuPrincipal;
    public GameObject menuCrear;
    public GameObject menuConectar;
    

    // Use this for initialization
    void Start () {
        OcultarTodos();
        menuPrincipal.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MenuPrincipal()
    {
        OcultarTodos();
        menuPrincipal.SetActive(true);
    }

    public void MenuCrear()
    {
        OcultarTodos();
        menuCrear.SetActive(true);
    }

    public void MenuUnirse()
    {
        OcultarTodos();
        menuConectar.SetActive(true);
        
    }

    public void BotonConectarAlServidor()
    {
        
        
    }

    void OcultarTodos()
    {
        menuPrincipal.SetActive(false);
        menuConectar.SetActive(false);
        menuCrear.SetActive(false);
    }

}
