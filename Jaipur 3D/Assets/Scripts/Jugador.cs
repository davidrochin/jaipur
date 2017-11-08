using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Jugador{

    public int fichas;
    public int sellosDeExcelencia;

    public void AgregarFichas(int cantidad) {
        fichas = fichas + cantidad;
    }

}
