using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Jugador{

    public int sumaFichas;
    public int fichasBonificacion;
    public int sellosDeExcelencia;

    public void SumarFicha(int cantidad) {
        sumaFichas = sumaFichas + cantidad;
    }

}
