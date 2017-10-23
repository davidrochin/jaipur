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

    }

    private void OnMouseDown() {

    }

    public void SetSeleccionada(bool param) {

    }

    public void SetMaterial(TipoMercancia material) {

    }

    void SetTextura() {

    }

    public enum TipoMercancia { Cuero, Especias, Tela, Plata, Oro, Diamante, Camello }
}