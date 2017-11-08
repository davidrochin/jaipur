using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Ficha : MonoBehaviour {

    public int id;

    public TipoFicha tipoFicha = TipoFicha.Cuero;
    public int valorFicha = 1;
    public Grupo grupo;

    private void OnTransformParentChanged() {
        grupo = GetComponentInParent<Grupo>();
    }

    public void EnviarAGrupo(Grupo gp) {
        transform.SetParent(gp.transform);
    }

    void ActualizarTextura() {
        string nombreMaterial = "cuero";
        switch (tipoFicha) {
            case TipoFicha.Cuero:
                nombreMaterial = "cuero";
                break;
            case TipoFicha.Especias:
                nombreMaterial = "especias";
                break;
            case TipoFicha.Tela:
                nombreMaterial = "tela";
                break;
            case TipoFicha.Plata:
                nombreMaterial = "plata";
                break;
            case TipoFicha.Oro:
                nombreMaterial = "oro";
                break;
            case TipoFicha.Diamante:
                nombreMaterial = "diamante";
                break;
            case TipoFicha.Camello:
                nombreMaterial = "camello";
                break;
            case TipoFicha.Bonus3:
                nombreMaterial = "bonus3";
                break;
            case TipoFicha.Bonus4:
                nombreMaterial = "bonus4";
                break;
            case TipoFicha.Bonus5:
                nombreMaterial = "bonus5";
                break;
        }

        foreach (Transform hijo in transform) {
            if (hijo.GetComponent<TextMesh>() == null) {
                hijo.GetComponent<Renderer>().material = Resources.Load("Fichas/" + nombreMaterial, typeof(Material)) as Material;
            }
        }
    }

    public void ActualizarApariencia() {
        ActualizarTextura();

        //Si es una ficha bonus, no actualizar el texto
        if (tipoFicha == TipoFicha.Camello || tipoFicha == TipoFicha.Bonus3 ||
                tipoFicha == TipoFicha.Bonus4 || tipoFicha == TipoFicha.Bonus5) {
            return;
        }

        foreach (Transform hijo in transform) {
            TextMesh texto = hijo.GetComponent<TextMesh>();
            if (texto != null) {
                texto.text = "" + valorFicha;
            }
        }
    }

    public enum TipoFicha { Cuero, Especias, Tela, Plata, Oro, Diamante, Camello, Bonus3, Bonus4, Bonus5 }
}
