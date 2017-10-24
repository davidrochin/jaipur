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
                FindObjectOfType<ManejadorJuego>().RemoverDeSeleccion(gameObject, true);
                SetSeleccionada(false);
            } else if (!seleccionada) {
                //Seleccionar
                if (FindObjectOfType<ManejadorJuego>().AgregarASeleccion(gameObject, true)) {
                    SetSeleccionada(true);
                }
            }

        }
    }

    public void SetSeleccionada(bool param) {
        seleccionada = param;
    }

    public void SetMercancia(TipoMercancia mercancia) {
        this.mercancia = mercancia;
        ActualizarTextura();
    }

    void ActualizarTextura() {

    }

    public enum TipoMercancia { Cuero, Especias, Tela, Plata, Oro, Diamante, Camello }
}