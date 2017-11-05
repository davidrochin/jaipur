using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugador : MonoBehaviour{

    public int fichas;
    public int sellosDeExcelencia;

    public void AgregarFichas(int cantidad) {
        fichas = fichas + cantidad;
    }

}
