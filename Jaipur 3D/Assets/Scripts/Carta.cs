using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Carta : MonoBehaviour {

    public TipoMercancia mercancia = TipoMercancia.Oro;
    public bool seleccionada = false;
    public int id = 0;

    public Grupo grupo;

    private void Awake() {
        //Asignar un material a la carta, conforme a su tipo de material.
        ActualizarTextura();
    }

    private void OnMouseDown() {
        //Tratar de agregar la carta a la selección si su Mazo lo permite
        if (GetComponentInParent<Grupo>().objetosSeleccionables) {

            //Revisar si se necesita De-seleccionar o Seleccionar
            if (seleccionada) {
                //De-seleccionar
                FindObjectOfType<Seleccion>().RemoverDeSeleccion(gameObject, true);
                SetSeleccionada(false);
            } else if (!seleccionada) {
                //Seleccionar
                if (FindObjectOfType<Seleccion>().AgregarASeleccion(gameObject, true)) {
                    SetSeleccionada(true);
                }
            }

        }
    }

    public void EnviarAGrupo(Grupo gp) {
        transform.SetParent(gp.transform);
    }

    public void SetSeleccionada(bool param) {
        seleccionada = param;
    }

    public void SetMercancia(TipoMercancia mercancia) {
        this.mercancia = mercancia;
        ActualizarTextura();
    }

    void ActualizarTextura() {
        string nombreArchivo = "cuero.mat";

        switch (mercancia) {
            case TipoMercancia.Cuero:
                nombreArchivo = "cuero";
                break;
            case TipoMercancia.Especias:
                nombreArchivo = "especias";
                break;
            case TipoMercancia.Tela:
                nombreArchivo = "tela";
                break;
            case TipoMercancia.Plata:
                nombreArchivo = "plata";
                break;
            case TipoMercancia.Oro:
                nombreArchivo = "oro";
                break;
            case TipoMercancia.Diamante:
                nombreArchivo = "diamante";
                break;
            case TipoMercancia.Camello:
                nombreArchivo = "camello";
                break;
        }

        foreach (Transform hijo in transform) {
            if (hijo.tag.Equals("Frente Carta")) {
                hijo.GetComponent<Renderer>().material = Resources.Load("Cartas/" + nombreArchivo, typeof(Material)) as Material;
            }
        }
    }

    public enum TipoMercancia { Cuero, Especias, Tela, Plata, Oro, Diamante, Camello }
}