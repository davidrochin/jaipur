using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Ficha : MonoBehaviour {

    public TipoFicha tipoFicha = TipoFicha.Cuero;
    public int valorFicha = 1;

    void Awake() {

    }

    void Update() {

    }

    void SetTextura() {
        
    }

    void SetValor() {
        
    }

    public void ActualizarApariencia() {
        
    }

    public enum TipoFicha { Cuero, Especias, Tela, Plata, Oro, Diamante, Camello, Bonus3, Bonus4, Bonus5 }
}
