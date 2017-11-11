using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Jugador{

    public int sumaFichas;
    public int fichasBonificacion;
    public int fichasProducto;
    public int sellosDeExcelencia;

    public void SumarFicha(int cantidad) {
        sumaFichas = sumaFichas + cantidad;
    }

    public void ResetearFichas() {
        sumaFichas = 0;
        fichasBonificacion = 0;
        fichasProducto = 0;
    }

}
