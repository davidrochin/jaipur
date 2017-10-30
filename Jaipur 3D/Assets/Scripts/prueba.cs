using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class prueba : MonoBehaviour {
    public InputField nombreInput;
    private Cliente c;

    // Use this for initialization
    void Start () {
        c = new Cliente();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void EnviarMensaje()
    {
        string dato = nombreInput.text;
        int[] carta = new int[0];
        Movimiento mov = new Movimiento(Movimiento.TipoMovimiento.Tomar, carta);
        c.EnviarMovimiento(mov);
    }
}
